using BotX.Api;
using BotX.Api.Extensions;
using BotX.Api.JsonModel;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
		private const string API_SEND_REPLY_MESSAGE = "api/v3/botx/command/callback";
		private const string API_SEND_MESSAGE_NOTIFICATION = "api/v3/botx/notification/callback";
		private const string API_SEND_EDIT_MESSAGE = "api/v3/botx/events/edit_event";
		private const string API_SEND_FILE = "api/v1/botx/file/callback";
		private const string API_GET_TOKEN = "api/v2/botx/bots/$bot_id$/token?signature=$hash$";

		private readonly HttpClient client;
		private readonly BotXConfig config;
		private readonly ILogger<BotXHttpClient> logger;

		private static string authToken = null;
		
		private static ManualResetEvent manualReset = new ManualResetEvent(true);
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

		public BotXHttpClient(HttpClient client, BotXConfig config, ILogger<BotXHttpClient> logger)
		{
			this.client = client;
			this.config = config;
			this.logger = logger;

			client.BaseAddress = new Uri(config.CtsServiceUrl);
			if (string.IsNullOrEmpty(config.SecretKey))
			{
				throw new ArgumentException($"SecretKey is required");
			}

			if (authToken == null)
				AuthorizeAsync().Wait();
		}

		public async Task SendNotificationAsync(NotificationMessage message)
		{
			await PostAsJsonAsync(API_SEND_MESSAGE_NOTIFICATION, message);			
		}

		public async Task<Guid> SendReplyAsync(ResponseMessage message)
		{
			var result = await PostAsJsonAsync(API_SEND_REPLY_MESSAGE, message);
			var syncId = JsonConvert.DeserializeObject<ReplyMessageResponse>(await result.Content.ReadAsStringAsync())?.Result?.SyncId ?? Guid.Empty;
			return syncId;
		}

		public async Task EditMessageAsync(EditEventMessage message)
		{
			await PostAsJsonAsync(API_SEND_EDIT_MESSAGE, message);
		}

		public async Task SendFileAsync(Guid syncId, Guid botId, string fileName, byte[] data)
		{
			if (botId == Guid.Empty)
				throw new InvalidOperationException("Для отправки файлов требуется задать идентификатор бота в AddExpressBot");
			
			var requestUri = new Uri(API_SEND_FILE);
			var content = new MultipartFormDataContent();
			content.Add(new StringContent(syncId.ToString()), "sync_id");
			content.Add(new StringContent(botId.ToString()), "bot_id");
			content.Add(new StreamContent(new MemoryStream(data)), "file", fileName);

			var response = await client.PostAsync(requestUri, content);
			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException(await response.Content.ReadAsStringAsync());
		}

		private async Task AuthorizeAsync()
		{
			// Блокируем отправку новых запросов, пока не закончится авторизация
			manualReset.Reset();
			logger.LogInformation("Autirozation");
			var key = Encoding.ASCII.GetBytes(config.SecretKey);
			var val = Encoding.ASCII.GetBytes(config.BotId.ToString());
			using (HMACSHA256 hmac = new HMACSHA256(key))
			{
				var hash = BitConverter.ToString(hmac.ComputeHash(val)).Replace("-", "");
				var url = API_GET_TOKEN.Replace("$bot_id$", config.BotId.ToString()).Replace("$hash$", hash);
				var resp = await client.GetAsync(url);
				if (resp.IsSuccessStatusCode)
				{
					var body = await resp.Content.ReadAsStringAsync();
					var data = JsonConvert.DeserializeObject<AuthResponse>(body);
					authToken = data.result;
				}
			}
			// Снимаем блокировку отправки запросов
			manualReset.Set();
		}

		/// <summary>
		/// Отправляет пост запрос к BotX API, при Unauthorized, повторно авторизется
		/// </summary>
		/// <exception cref="HttpRequestException">Возникает при не успешном ответе апи</exception>
		private async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			manualReset.WaitOne();
			var dataAsString = JsonConvert.SerializeObject(data);
			logger.LogInformation($"POST: {url}\r\nDATA: {dataAsString}");

			var response = await client.SendAsync(CreateRequestMessage(url, dataAsString));
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// Если сразу на нескольких запросах получен Unauthorized, запускаем авторизацию только 1 раз
				await semaphoreSlim.WaitAsync();
				try { 
					if (authToken == response.RequestMessage.Headers.Authorization.Parameter)
						await AuthorizeAsync();
				}
				finally
				{
					semaphoreSlim.Release();
				}
				response = await client.SendAsync(CreateRequestMessage(url, dataAsString));
			}
			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException(await response.Content.ReadAsStringAsync());

			return response;
		}

		private HttpRequestMessage CreateRequestMessage(string url, string data)
		{
			var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
			requestMessage.Content = new StringContent(data);
			requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return requestMessage;
		}
	}
}
