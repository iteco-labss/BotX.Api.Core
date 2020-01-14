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
		private const string API_VERSION_2 = "v2";
		private const string API_VERSION_3 = "v3";
		private const string API_SEND_MESSAGE_ASNWER = "botx/command/callback";
		private const string API_SEND_MESSAGE_NOTIFICATION = "botx/notification/callback";
		private const string API_SEND_FILE = "v1/botx/file/callback";
		private const string API_GET_TOKEN = "v2/botx/bots/$bot_id$/token?signature=$hash$";

		private readonly string apiVersion;
		private readonly HttpClient client;
		private readonly BotXConfig config;
		private readonly ILogger<BotXHttpClient> logger;

		static string authToken = null;

		public BotXHttpClient(HttpClient client, BotXConfig config, ILogger<BotXHttpClient> logger)
		{
			this.client = client;
			this.config = config;
			this.logger = logger;
			this.apiVersion = !string.IsNullOrEmpty(config.SecretKey) ? API_VERSION_3 : API_VERSION_2;

			client.BaseAddress = new Uri(new Uri(config.CtsServiceUrl), "api/");

			if (authToken == null && !string.IsNullOrEmpty(config.SecretKey))
				AuthorizeAsync().Wait();


		}

		public async Task SendNotificationAsync(NotificationMessage message)
		{
			ValidateMessage(message);
			var requestUrl = $"{apiVersion}/{API_SEND_MESSAGE_NOTIFICATION}";
			var result = await PostAsJsonAsync(requestUrl, message);
			if (!result.IsSuccessStatusCode)
				throw new HttpRequestException(await result.Content.ReadAsStringAsync());
		}

		public async Task SendReplyAsync(ResponseMessage message)
		{
			ValidateMessage(message);
			var requestUrl = $"{apiVersion}/{API_SEND_MESSAGE_ASNWER}";
			var result = await PostAsJsonAsync(requestUrl, message);
			if (!result.IsSuccessStatusCode)
				throw new HttpRequestException(await result.Content.ReadAsStringAsync());
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

			var result = await client.PostAsync(requestUri, content);
			if (!result.IsSuccessStatusCode)
				throw new HttpRequestException(await result.Content.ReadAsStringAsync());
		}

		private async Task AuthorizeAsync()
		{
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
		}

		private async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var dataAsString = JsonConvert.SerializeObject(data);
			logger.LogInformation(url);
			logger.LogInformation("sending... " + dataAsString);

			var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
			requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
			requestMessage.Content = new StringContent(dataAsString);
			requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			var response = await client.SendAsync(requestMessage);
			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				await AuthorizeAsync();
				requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
				return await client.SendAsync(requestMessage);
			}

			return response;
		}

		private void ValidateMessage(IMessage message)
		{
			if (message.BotId == Guid.Empty)
				throw new InvalidOperationException(
					$"Для отправки сообщений пользователю, необходимо задать идентификатор бота " +
					$"(метод {nameof(ServiceCollectionExtension.AddExpressBot)}). " +
					$"Без идентификатора возможно только получение и ответ на полученные сообщения");
		}
	}
}
