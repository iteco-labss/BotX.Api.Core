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
		public async Task EditMessageAsync(Guid syncId, string messageText)
		{
			await EditMessageInternalAsync(
				syncId: syncId,
				messageText: messageText,
				buttons: null,
				mentions: null
				);
		}

		public async Task EditMessageAsync(Guid syncId, string messageText, MessageButtonsGrid buttons)
		{
			await EditMessageInternalAsync(
				syncId: syncId,
				messageText: messageText,
				buttons: buttons,
				mentions: null
				);
		}

		public async Task EditMessageAsync(Guid syncId, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			await EditMessageInternalAsync(
				syncId: syncId,
				messageText: messageText,
				buttons: null,
				mentions: mentions
				);
		}
		private async Task EditMessageInternalAsync(Guid syncId, string messageText, MessageButtonsGrid buttons, Mention[] mentions)
		{
			await httpClient.EditMessageAsync(new EditEventMessage
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
