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

		#region Отправка текстового сообщения
		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="chatIds">Идентификаторы чатов, куда будет отправлено сообщение</param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText)
		{
			await SendTextMessageAsync(
				chatIds: chatIds,
				recipients: recipients,
				messageText: messageText,
				buttons: new MessageButtonsGrid());
		}

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="chatIds">Идентификаторы чатов, куда будет отправлено сообщение</param>
		/// <param name="recipients">Идентификаторы получателей (пользователей) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText, MessageButtonsGrid buttons)
		{
			var notification = new NotificationMessage
			{
				BotId = ExpressBotService.Configuration.BotId,
				GroupChatIds = chatIds,
				Recipients = recipients,
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			};


			await SendTextMessageAsync(notification);
		}

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) пользователю
		/// </summary>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="recipient">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid chatId, Guid recipient, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				recipient: recipient,
				messageText: messageText,
				buttons: new MessageButtonsGrid());
		}

		/// <summary>
		/// Отправляет текстовое сообщение (нотификацию) с кнопками пользователю
		/// </summary>
		/// <param name="chatId">Идентификатор чата, куда будет отправлено сообщение</param>
		/// <param name="recipient">Идентификатор получателя (пользователя) сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task SendTextMessageAsync(Guid chatId, Guid recipient, string messageText, MessageButtonsGrid buttons)
		{
			var notification = new NotificationMessage
			{
				BotId = ExpressBotService.Configuration.BotId,
				GroupChatIds = new Guid[] { chatId },
				Recipients = new Guid[] { recipient },
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			};


			await SendTextMessageAsync(notification);
		}

		internal async Task SendTextMessageAsync(NotificationMessage message)
		{
			ValidateMessage(message);
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

		#endregion

		#region Отправка ответа на сообщение от пользователя
		/// <summary>
		/// Отправляет ответ в виде текстового сообщения
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">Адресат сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <returns></returns>
		public async Task ResponseTextMessageAsync(Guid syncId, Guid to, string messageText)
		{
			await ResponseTextMessageInternalAsync(
				botId: ExpressBotService.Configuration.BotId,
				syncId: syncId,
				to: to,
				messageText: messageText);
		}

		/// <summary>
		/// Отправляет текствое сообщение с кнопками в ответ пользователю
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="to">Адресат сообщения</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task ResponseTextMessageAsync(Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons)
		{
			await ResponseTextMessageWithButtonsAsync(
				botId: ExpressBotService.Configuration.BotId,
				syncId: syncId,
				to: to,
				messageText: messageText,
				buttons: buttons
				);
		}
		
		/// <summary>
		/// Отправляет текстовое сообщение в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение от пользователя</param>
		/// <param name="messageText">Текст ответа</param>
		/// <returns></returns>
		public async Task ResponseTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			await ResponseTextMessageInternalAsync(
				botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				to: requestMessage.From.Huid,
				messageText: messageText
				);
		}

		/// <summary>
		/// Отправка текстового сообщения с кнопками (действиями) в ответ пользователю
		/// </summary>
		/// <param name="requestMessage">Сообщение пользователя</param>
		/// <param name="messageText">Текст сообщения</param>
		/// <param name="buttons">Кнопки с действиями в сообщении</param>
		/// <returns></returns>
		public async Task ResponseTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons)
		{
			await ResponseTextMessageWithButtonsAsync(
					botId: requestMessage.BotId,
				   syncId: requestMessage.SyncId,
				   to: requestMessage.From.Huid,
				   messageText: messageText,
				   buttons: buttons
				   );
		}
		private async Task ResponseTextMessageWithButtonsAsync(Guid botId, Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons)
		{
			if (botId == Guid.Empty)
				return;

			await SendTextMessageAsync(new ResponseMessage
			{
				BotId = botId,
				SyncId = syncId,
				Recipient = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			});
		}

		// TODO Понять насколько нужен этот метод, возможно стоит вызывать просто 
		// private async Task ResponseTextMessageWithButtonsAsync(Guid botId, Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons)
		private async Task ResponseTextMessageInternalAsync(Guid botId, Guid syncId, Guid to, string messageText)
		{
			if (botId == Guid.Empty)
				return;

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


		#endregion

		#region Отправка файла
		/// <summary>
		/// Отправка файла в чат
		/// </summary>
		/// <param name="syncId">Идентификатор чата</param>
		/// <param name="fileName">Имя фалйа</param>
		/// <param name="data">Данные файла</param>
		/// <returns></returns>
		public async Task SendFileAsync(Guid syncId, string fileName, byte[] data)
		{
			await SendFileMessageAsync(
				syncId: syncId,
				botId: ExpressBotService.Configuration.BotId,
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
				syncId: requestMessage.SyncId,
				fileName: fileName,
				data: data
				);
		}
		internal async Task SendFileMessageAsync(Guid syncId, Guid botId, string fileName, byte[] data)
		{
			if (botId == Guid.Empty)
				throw new InvalidOperationException("Для отправки файлов требуется задать идентификатор бота в AddExpressBot");

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
		#endregion

		internal async Task SendTextMessageAsync(ResponseMessage message)
		{
			ValidateMessage(message);
			logger.LogInformation("sending... " + JsonConvert.SerializeObject(message));
			using (HttpClient client = new HttpClient())
			{
				var requestUri = new Uri(new Uri(server), API_SEND_MESSAGE_ASNWER);
				logger.LogInformation(requestUri.ToString());
				var result = await client.PostAsJsonAsync(requestUri, message);
				if (!result.IsSuccessStatusCode)
					throw new HttpRequestException(await result.Content.ReadAsStringAsync());
			}
		}




		private void ValidateMessage(ResponseMessage message)
		{
			if (message.BotId == Guid.Empty)
				throw new InvalidOperationException("Для отправки сообщений пользователю, необходимо задать идентификатор бота (метод AddExpressBot). Без идентификатора возможно только получение и ответ на полученные сообщения");
		}

		private void ValidateMessage(NotificationMessage message)
		{
			if (message.BotId == Guid.Empty)
				throw new InvalidOperationException("Для отправки сообщений пользователю, необходимо задать идентификатор бота (метод AddExpressBot). Без идентификатора возможно только получение и ответ на полученные сообщения");
		}
	}
}
