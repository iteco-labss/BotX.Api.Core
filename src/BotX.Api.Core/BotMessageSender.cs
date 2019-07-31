using BotX.Api.Abstract;
using BotX.Api.BotUI;
using BotX.Api.Extensions;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BotX.Api
{
	/// <summary>
	/// Клиент, реализующий отправку сообщений, используя BotX Api
	/// </summary>
	public class BotMessageSender
    {
        private const string API_SEND_MESSAGE_ASNWER = "api/v2/botx/command/callback";
		private const string API_SEND_MESSAGE_NOTIFICATION = "/api/v2/botx/notification/callback";
        private const string API_SEND_FILE = "api/v1/botx/file/callback";
        private readonly ILogger<BotMessageSender> logger;
		private readonly string server;

		/// <summary>
		/// Инициализация нового экземпляра клиента
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="server"></param>
		public BotMessageSender(ILogger<BotMessageSender> logger, string server)
        {
            this.logger = logger;
			this.server = server;
		}

		/// <summary>
		/// Отправляет текстовое сообщение в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="messageText">Текст ответа</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			await SendTextMessageAsync(
				botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				to: requestMessage.From.Huid,
				messageText: messageText
				);
		}

		public async Task SendTextMessageAsync(Guid botId, Guid[] chatIds, Guid[] recipients, string messageText)
		{
			var notification = new NotificationMessage
			{
				BotId = botId,
				GroupChatIds = chatIds,
				Recipients = recipients,
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText
				}
			};

			await SendTextMessageAsync(notification);
		}

		/// <summary>
		/// Отправляет текстовое сообщение
		/// </summary>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">Адресат сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid botId, Guid syncId, Guid to, string messageText)
		{
			await SendTextMessageAsync(new ResponseMessage
			{
				BotId = botId,
				SyncId = syncId,
				Recipient = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText
				}
			});
		}

		/// <summary>
		/// Отправляет текствое сообщение с кнопками
		/// </summary>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">Адресат сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid botId, Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons)
		{
			var bubbles = buttons.Rows.Select(
				x => x.Buttons.Select(
					btn =>
					new Bubble
					{
						Command = btn.InternalCommand,
						Label = btn.Title
					}));

			await SendTextMessageAsync(new ResponseMessage
			{
				BotId = botId,
				SyncId = syncId,
				Recipient = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = bubbles
				}
			});
		}

		/// <summary>
		/// Отправка текстового сообщения с кнопками (действиями) в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение пользователя</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons)
		{
			await SendTextMessageAsync(
				   botId: requestMessage.BotId,
				   syncId: requestMessage.SyncId,
				   to: requestMessage.From.Huid,
				   messageText: messageText,
				   buttons: buttons
				   );
		}

		/// <summary>
		/// Отправка файла в чат
		/// </summary>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="fileName">Имя фалйа</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>
		public async Task SendFileAsync(Guid botId, Guid syncId, string fileName, byte[] data)
		{
			string base64 = Convert.ToBase64String(data);
			await SendFileMessageAsync(
				syncId: syncId,
				botId: botId,
				fileName: fileName,
				data: data
				);
		}


		/// <summary>
		/// Отправляет файл в ответ на пользовательское сообщение
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="fileName">Имя файла</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>
		public async Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data)
		{
			await SendFileAsync(
				botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				fileName: fileName,
				data: data
				);
		}

		internal async Task SendTextMessageAsync(ResponseMessage message)
        {
            logger.LogInformation("sending... " + JsonConvert.SerializeObject (message));
            using (HttpClient client = new HttpClient())
            {
                var requestUri = new Uri(new Uri(server), API_SEND_MESSAGE_ASNWER);
                logger.LogInformation(requestUri.ToString());
                var result = await client.PostAsJsonAsync(requestUri, message);
                if (!result.IsSuccessStatusCode)
                    throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
        }

		internal async Task SendTextMessageAsync(NotificationMessage message)
		{
			logger.LogInformation("sending... " + JsonConvert.SerializeObject(message));
			using (HttpClient client = new HttpClient())
			{
				var requestUri = new Uri(new Uri(server), API_SEND_MESSAGE_NOTIFICATION);
				logger.LogInformation(requestUri.ToString());
				var result = await client.PostAsJsonAsync(requestUri, message);
				if (!result.IsSuccessStatusCode)
					throw new HttpRequestException(await result.Content.ReadAsStringAsync());
			}
		}

        internal async Task SendFileMessageAsync(Guid syncId, Guid botId, string fileName, byte[] data)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestUri = new Uri(new Uri(server), API_SEND_FILE);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(syncId.ToString()), "sync_id");
                content.Add(new StringContent(botId.ToString()), "bot_id");
				content.Add(new StreamContent(new MemoryStream(data)), "file", fileName);

				var result = await client.PostAsync(requestUri, content);
				if (!result.IsSuccessStatusCode)
					throw new HttpRequestException(await result.Content.ReadAsStringAsync());
            }
        }
    }
}
