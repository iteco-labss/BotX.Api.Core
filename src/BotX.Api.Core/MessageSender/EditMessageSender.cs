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
				messageText: messageText);
		}

		public async Task EditMessageAsync(Guid syncId, string messageText, MessageButtonsGrid buttons)
		{
			await EditMessageInternalAsync(
				syncId: syncId,
				messageText: messageText,
				buttons: buttons
				);
		}

		public async Task EditMessageAsync(Guid syncId, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			await EditMessageInternalAsync(
				syncId: syncId,
				messageText: messageText,
				mentions: mentions
				);
		}
		private async Task EditMessageInternalAsync(Guid syncId, string messageText, MessageButtonsGrid buttons = null, Mention[] mentions = null)
		{
			await httpClient.EditMessageAsync(new EditEventMessage
			{
				SyncId = syncId,
				Payload = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons?.GridType == MessageButtonsGridType.Bubble
						? buttons?.ToButtonsOfType<Bubble>()
						: null
							?? new List<List<Bubble>>(),
					Keyboard = buttons?.GridType == MessageButtonsGridType.Keyboard
						? buttons?.ToButtonsOfType<Keyboard>()
						: null
							?? new List<List<Keyboard>>(),
					Mentions = mentions
				}
			});
		}
	}
}
