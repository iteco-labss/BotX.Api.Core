using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api
{
	internal partial class BotMessageSender : IBotMessageSender
	{
		public async Task SendTextMessageAsync(Guid chatId, Guid huid, Guid messageSyncId, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				recipients: new Guid[] { huid },
				messageSyncId: messageSyncId,
				messageText: messageText,
				buttons: new MessageButtonsGrid()
			);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, Guid messageSyncId, string messageText, MessageButtonsGrid buttons)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				recipients: new Guid[] { huid },
				messageSyncId: messageSyncId,
				messageText: messageText,
				buttons: buttons
			);

		}
		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId ,
				recipients: new Guid[] { huid },
				messageSyncId: null,
				messageText: messageText,
				buttons: new MessageButtonsGrid()
				);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons)
		{
			await SendTextMessageAsync(
				chatId: chatId ,
				recipients: new Guid[] { huid },
				messageSyncId: null,
				messageText: messageText,
				buttons: buttons
				);

		}
		public async Task SendTextMessageAsync(Guid chatId, Guid[] recipients, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				recipients: recipients,
				messageSyncId: null,
				messageText: messageText,
				buttons: new MessageButtonsGrid()
				);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid[] recipients, Guid? messageSyncId, string messageText, MessageButtonsGrid buttons)
		{
			var notification = new NotificationMessage
			{
				GroupChatId = chatId,
				Recipients = recipients,
				MessageSyncId = messageSyncId,
				Notification = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons.GetBubbles() ?? new List<List<Bubble>>()
				}
			};

			await httpClient.SendNotificationAsync(notification);
		}
	}
}
