using BotX.Api.Configuration;
using BotX.Api.Extensions;
using BotX.Api.JsonModel;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
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

		public BotXHttpClient(HttpClient client, BotXConfig config, ILogger<BotXHttpClient> logger)
		{
			this.client = client;
			this.config = config;
			this.logger = logger;
			this.apiVersion = !string.IsNullOrEmpty(config.SecretKey) ? API_VERSION_3 : API_VERSION_2;

			client.BaseAddress = new Uri(new Uri(config.CtsServiceUrl), "api/");

			if (config.AuthToken != null)
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.AuthToken);
			//else
			//	AuthorizeAsync().Wait();


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
			// TODO Вроде здесь реплай и у нас уже есть бот ид
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

		public async Task AuthorizeAsync()
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
					var data = JsonConvert.DeserializeObject<AuthResponce>(body);
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.result);
					config.AuthToken = data.result;
				}
			}
		}

		private Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T data)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			if (data == null)
				throw new ArgumentNullException(nameof(data));

			var dataAsString = JsonConvert.SerializeObject(data);
			logger.LogInformation(url);
			logger.LogInformation("sending... " + dataAsString);
			var content = new StringContent(dataAsString);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			return client.PostAsync(url, content);
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

	public interface IBotXHttpClient
	{
		public Task AuthorizeAsync();
		public Task SendFileAsync(Guid syncId, Guid botId, string fileName, byte[] data);
		public Task SendReplyAsync(ResponseMessage message);
		public Task SendNotificationAsync(NotificationMessage message);
	}

	internal class AuthResponce
	{
		public string status { get; set; }
		public string result { get; set; }
	}

	public class CheckUnauthorizeHandler : DelegatingHandler
	{
		//private readonly IBotXHttpClient client;

		//public CheckUnauthorizeHandler(IBotXHttpClient client)
		//{
		//	this.client = client;
		//}

		protected override async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			var response = await base.SendAsync(request, cancellationToken);

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				//await client.AuthorizeAsync();
				return await base.SendAsync(request, cancellationToken);
			}
			return response;
		}
	}
}
