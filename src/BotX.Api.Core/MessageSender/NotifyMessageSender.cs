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
		public async Task SendTextMessageAsync(Guid[] chatIds, Guid[] recipients, string messageText)
		{
			await SendTextMessageAsync(
				chatIds: chatIds,
				recipients: recipients,
				messageText: messageText,
				buttons: new MessageButtonsGrid());
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


			await httpClient.SendNotificationAsync(notification);
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText)
		{
			await SendTextMessageAsync(
				chatId: chatId,
				huid: huid,
				messageText: messageText,
				buttons: new MessageButtonsGrid());
		}

		public async Task SendTextMessageAsync(Guid chatId, Guid huid, string messageText, MessageButtonsGrid buttons)
		{
			var notification = new NotificationMessage
			{
				BotId = ExpressBotService.Configuration.BotId,
				GroupChatIds = new Guid[] { chatId },
				Recipients = new Guid[] { huid },
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
