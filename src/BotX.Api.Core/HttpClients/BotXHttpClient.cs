using BotX.Api;
using BotX.Api.Configuration;
using BotX.Api.Extensions;
using BotX.Api.JsonModel;
using BotX.Api.JsonModel.Api.Request;
using BotX.Api.JsonModel.Api.Response;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotX.Api.HttpClients
{
	internal class BotXHttpClient : IBotXHttpClient
    {
        private const string API_SEND_REPLY_MESSAGE = "/api/v4/botx/notifications/direct";
		private const string API_SEND_MESSAGE_NOTIFICATION = "api/v3/botx/notification/callback/direct";
		private const string API_SEND_EDIT_MESSAGE = "api/v3/botx/events/edit_event";
		private const string API_UPLOAD_FILE = "/api/v3/botx/files/upload";
		private const string API_GET_TOKEN = "api/v2/botx/bots/$bot_id$/token?signature=$hash$";

		private readonly HttpClient client;
		private readonly BotXConfig config;
		private readonly ILogger<BotXHttpClient> logger;

		private static Hashtable authTokens = new Hashtable();

		private static ManualResetEvent manualReset = new ManualResetEvent(true);
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

		public BotXHttpClient(HttpClient client, BotXConfig config, ILogger<BotXHttpClient> logger)
		{
			this.client = client;
			this.config = config;
			this.logger = logger;
		}

		public async Task SendNotificationAsync(Guid botId, NotificationMessage message)
		{
			await PostAsJsonAsync(botId, API_SEND_MESSAGE_NOTIFICATION, message);
		}

		public async Task<Guid> SendReplyAsync(Guid botId, ResponseMessage message)
		{
			var result = await PostAsJsonAsync(botId, API_SEND_REPLY_MESSAGE, message);
			var syncId = JsonConvert.DeserializeObject<ReplyMessageResponse>(await result.Content.ReadAsStringAsync())?.Result?.SyncId ?? Guid.Empty;
			return syncId;
		}

		public async Task EditMessageAsync(Guid botId, EditEventMessage message)
		{
			await PostAsJsonAsync(botId,API_SEND_EDIT_MESSAGE, message);
		}

		public async Task<FileMetadataResult> UploadFileAsync(Guid botId, Guid chatId, string fileName, byte[] data, string mimeType, FileMetaInfo meta)
		{
			var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(new MemoryStream(data));
			fileContent.Headers.Add("Content-Disposition", $"form-data; name=\"content\"; filename=\"{fileName}\"");
            fileContent.Headers.Add("Content-Type", mimeType);
			content.Add(fileContent);

			content.Add(new StringContent(chatId.ToString()), "\"group_chat_id\"");

			content.Add(new StringContent(JsonConvert.SerializeObject(meta, Formatting.Indented), Encoding.UTF8, "application/json"), "\"meta\"");

			var response = await PostAsMultipartAsync(botId, API_UPLOAD_FILE, content);

			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException(await response.Content.ReadAsStringAsync());

			return JsonConvert.DeserializeObject<FileMetadataResult>(await response.Content.ReadAsStringAsync());
		}

		private async Task AuthorizeAsync(Guid botId)
		{
			// Блокируем отправку новых запросов, пока не закончится авторизация
			manualReset.Reset();
			logger.LogInformation("Authentication");

			var botEntry = config.GetEntryBy(botId);

			var key = Encoding.ASCII.GetBytes(botEntry.Secret);
			var val = Encoding.ASCII.GetBytes(botEntry.BotId.ToString());
			using (HMACSHA256 hmac = new HMACSHA256(key))
			{
				var hash = BitConverter.ToString(hmac.ComputeHash(val)).Replace("-", "");
				var url = API_GET_TOKEN.Replace("$bot_id$", botEntry.BotId.ToString()).Replace("$hash$", hash);
				var resp = await client.GetAsync(new Uri(botEntry.Cts, url));
				if (resp.IsSuccessStatusCode)
				{
					var body = await resp.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<AuthResponse>(body);
					authTokens[botEntry.BotId] = data.result;
				}
			}
			// Снимаем блокировку отправки запросов
			manualReset.Set();
		}

		/// <summary>
		/// Отправляет Json пост запрос к BotX API, при Unauthorized, повторно авторизется
		/// </summary>
		/// <exception cref="HttpRequestException">Возникает при не успешном ответе апи</exception>
		private async Task<HttpResponseMessage> PostAsJsonAsync<T>(Guid botId, string url, T data)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			manualReset.WaitOne();
			var dataAsString = JsonConvert.SerializeObject(data);
			logger.LogDebug($"POST: {url}\r\nDATA: {dataAsString}");

			var response = await client.SendAsync(CreateRequestMessage(botId, url, new StringContent(dataAsString), "application/json"));
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Если сразу на нескольких запросах получен Unauthorized, запускаем авторизацию только 1 раз
				await semaphoreSlim.WaitAsync();
				try
				{
					await AuthorizeAsync(botId);
				}
				finally
				{
					semaphoreSlim.Release();
				}
				response = await client.SendAsync(CreateRequestMessage(botId, url, new StringContent(dataAsString), "application/json"));
			}
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				logger.LogError($"Request Exception: {response.StatusCode} - {content}");
				throw new HttpRequestException(content);
			}

			return response;
		}

		/// <summary>
		/// Отправляет Multipart пост запрос к BotX API, при Unauthorized, повторно авторизется
		/// </summary>
		/// <exception cref="HttpRequestException">Возникает при не успешном ответе апи</exception>
		private async Task<HttpResponseMessage> PostAsMultipartAsync(Guid botId, string url, MultipartFormDataContent data)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			manualReset.WaitOne();
			logger.LogDebug($"POST: {url}\r\nDATA: {await data.ReadAsStringAsync()}");

			var response = await client.SendAsync(CreateRequestMessage(botId, url, data));
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Если сразу на нескольких запросах получен Unauthorized, запускаем авторизацию только 1 раз
				await semaphoreSlim.WaitAsync();
				try
				{
					await AuthorizeAsync(botId);
				}
				finally
				{
					semaphoreSlim.Release();
				}
				response = await client.SendAsync(CreateRequestMessage(botId, url, data));
			}
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				logger.LogError($"Request Exception: {response.StatusCode} - {content}");
				throw new HttpRequestException(content);
			}

			return response;
		}

		private HttpRequestMessage CreateRequestMessage(Guid botId, string path, HttpContent data, string encoding = null)
		{
			var entry = config.GetEntryBy(botId);
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(entry.Cts, path));
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authTokens[botId]?.ToString());
			requestMessage.Content = data;

			if (encoding != null) requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(encoding);

			return requestMessage;
		}
	}
}
