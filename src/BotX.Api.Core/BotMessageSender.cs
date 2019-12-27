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
using BotX.Api.JsonModel;
using BotX.Api.JsonModel.Response.Mentions;

namespace BotX.Api
{
	internal class BotMessageSender : IBotMessageSender
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
		internal BotMessageSender(ILogger<BotMessageSender> logger, string server)
		{
			this.logger = logger;
			this.server = server;
		}

		#region Отправка текстового сообщения
		
		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText)
		{
			await SendTextMessageAsync(chatIds, recipients, messageText, ExpressBotService.Configuration.BotId, string.Empty);
		}
		
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

			await PostNotificationAsync(notification, null);
		}

		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText, MessageButtonsGrid buttons, Guid botId, string cts)
		{
			var notification = new NotificationMessage
			{
				BotId = botId,
				GroupChatIds = chatIds,
				Recipients = recipients,
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			};

			await PostNotificationAsync(notification, cts);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				huid: huid,
				messageText: messageText,
				buttons: new MessageButtonsGrid(), 
				botId: ExpressBotService.Configuration.BotId,
				cts:string.Empty);
		}
		
		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				huid: huid,
				messageText: messageText,
				buttons: buttons,
				botId: ExpressBotService.Configuration.BotId,
				cts: string.Empty);
		}

		internal async Task PostNotificationAsync(NotificationMessage message, string cts)
		{
			ValidateMessage(message);
			if (string.IsNullOrEmpty(cts))
				cts = server;
			logger.LogInformation("sending... " + JsonConvert.SerializeObject(message));
			using (HttpClient client = new HttpClient())
			{
				var requestUri = new Uri(new Uri(cts), API_SEND_MESSAGE_NOTIFICATION);
				logger.LogInformation(requestUri.ToString());
				var result = await client.PostAsJsonAsync(requestUri, message);
				if (!result.IsSuccessStatusCode)
					throw new HttpRequestException(await result.Content.ReadAsStringAsync());
			}
		}

		#endregion

		#region Отправка ответа на сообщение от пользователя
		
		public async Task ReplyTextMessageAsync(Guid syncId, Guid to, string messageText)
		{
			await ReplyTextMessageInternalAsync(
				botId: ExpressBotService.Configuration.BotId,
				syncId: syncId,
				to: to,
				messageText: messageText,
				server: this.server);
		}
		
		public async Task ReplyTextMessageAsync(Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons)
		{
			await ReplyTextMessageAsync(
				botId: ExpressBotService.Configuration.BotId,
				syncId: syncId,
				to: to,
				messageText: messageText,
				buttons: buttons,
				server: this.server
				);
		}
		
		public async Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			await ReplyTextMessageInternalAsync(
				botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				to: requestMessage.From.Huid,
				messageText: messageText,
				server: requestMessage.From.Host
				);
		}
		
		public async Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			await ReplyTextMessageInternalAsync(
				botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				to: requestMessage.From.Huid,
				messageText: messageText,
				mentions: mentions,
				server: requestMessage.From.Host
				);
		}
		
		public async Task ReplyTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons)
		{
			await ReplyTextMessageAsync(
					botId: requestMessage.BotId,
				   syncId: requestMessage.SyncId,
				   to: requestMessage.From.Huid,
				   messageText: messageText,
				   buttons: buttons,
				   server: requestMessage.From.Host
				   );
		}

		private async Task ReplyTextMessageAsync(Guid botId, Guid syncId, Guid to, string messageText, MessageButtonsGrid buttons, string server)
		{
			if (botId == Guid.Empty)
				return;

			await PostReplyAsync(new ResponseMessage
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
			}, server);
		}

		private async Task ReplyTextMessageInternalAsync(Guid botId, Guid syncId, Guid to, string messageText, string server)
		{
			if (botId == Guid.Empty)
				return;

			await PostReplyAsync(new ResponseMessage
			{
				BotId = botId,
				SyncId = syncId,
				Recipient = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText
				}
			}, server);
		}

		private async Task ReplyTextMessageInternalAsync(Guid botId, Guid syncId, Guid to, string messageText, Mention[] mentions, string server)
		{
			if (botId == Guid.Empty)
				return;

			await PostReplyAsync(new ResponseMessage
			{
				BotId = botId,
				SyncId = syncId,
				Recipient = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Mentions = mentions
				}
			}, server);
		}

		#endregion

		#region Отправка файла
		
		public async Task SendFileAsync(Guid syncId, string fileName, byte[] data)
		{
			await PostFileAsync(
				syncId: syncId,
				botId: ExpressBotService.Configuration.BotId,
				fileName: fileName,
				data: data
				);
		}
				
		public async Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data)
		{
			await SendFileAsync(
				syncId: requestMessage.SyncId,
				fileName: fileName,
				data: data
				);
		}

		internal async Task PostFileAsync(Guid syncId, Guid botId, string fileName, byte[] data)
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

		internal async Task PostReplyAsync(ResponseMessage message, string server)
		{
			ValidateMessage(message);
			logger.LogInformation("sending... " + JsonConvert.SerializeObject(message));
			using (HttpClient client = new HttpClient())
			{
				//TODO: отрефакторить работу с переданным сервером. Они приходят в разных форматах (один с протоколом, другой нет)
				string apiServer = string.IsNullOrEmpty(server) ? this.server : $"https://{server}";
				var requestUri = new Uri(new Uri(apiServer), API_SEND_MESSAGE_ASNWER);
				logger.LogInformation(requestUri.ToString());
				var result = await client.PostAsJsonAsync(requestUri, message);
				if (!result.IsSuccessStatusCode)
					throw new HttpRequestException(await result.Content.ReadAsStringAsync());
			}
		}

		private void ValidateMessage(IMessage message)
		{
			if (message.BotId == Guid.Empty)
				throw new InvalidOperationException(
					$"Для отправки сообщений пользователю, необходимо задать идентификатор бота " +
					$"(метод {nameof(ServiceCollectionExtension.AddExpressBot)}). " +
					$"Без идентификатора возможно только получение и ответ на полученные сообщения");
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, Guid botId, string cts)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				huid: huid,
				messageText: messageText,
				buttons: new MessageButtonsGrid(), botId, cts);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons, Guid botId, string cts)
		{
			var notification = new NotificationMessage
			{
				BotId = botId,
				GroupChatIds = new Guid[] { chatId },
				Recipients = new Guid[] { huid },
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			};

			await PostNotificationAsync(notification, null);
		}

		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText, Guid botId, string cts)
		{
			await SendTextMessageAsync(
				chatIds: chatIds,
				recipients: recipients,
				messageText: messageText,
				buttons: new MessageButtonsGrid(),
				botId: botId,
				cts);
		}
	}
}
