using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using BotX.Api.JsonModel.Response.Mentions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api
{
	internal partial class BotMessageSender : IBotMessageSender
	{
		public async Task EditMessageAsync(UserMessage requestMessage, Guid syncId, string messageText)
		{
			await EditMessageInternalAsync(
				botId: requestMessage.BotId,
				syncId: syncId,
				messageText: messageText,
				buttons: null, 
				mentions: null
				);
		}

		public async Task EditMessageAsync(UserMessage requestMessage, Guid syncId, string messageText, MessageButtonsGrid buttons)
		{
			await EditMessageInternalAsync(
				botId: requestMessage.BotId,
				syncId: syncId,
				messageText: messageText,
				buttons: buttons,
				mentions: null
				);
		}

		public async Task EditMessageAsync(Guid botId, Guid syncId, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			await EditMessageInternalAsync(
				botId: botId,
				syncId: syncId,
				messageText: messageText,
				buttons: null,
				mentions: mentions
				);
		}
		private async Task EditMessageInternalAsync(Guid botId, Guid syncId, string messageText, MessageButtonsGrid buttons, Mention[] mentions)
		{
			await httpClient.EditMessageAsync(botId, new EditEventMessage
			{
				SyncId = syncId,
				Payload = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons?.GetBubbles() ?? new List<List<Bubble>>(),
					Mentions = mentions
				}
			});
		}
	}
}
