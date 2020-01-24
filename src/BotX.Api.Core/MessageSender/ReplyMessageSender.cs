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
		public async Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid[] to, string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: syncId,
				to: to,
				messageText: messageText);
		}

		public async Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid[] to, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageAsync(
				syncId: syncId,
				to: to,
				messageText: messageText,
				buttons: buttons
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageText: messageText
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageText: messageText,
				mentions: mentions
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageInternalAsync(
				   syncId: requestMessage.SyncId,
				   messageText: messageText,
				   buttons: buttons
				   );
		}
		private async Task<Guid> ReplyTextMessageInternalAsync(Guid syncId, string messageText, Guid[] to = null, MessageButtonsGrid buttons = null, Mention[] mentions = null)
		{
			return await httpClient.SendReplyAsync(new ResponseMessage
			{
				SyncId = syncId,
				Recipients = to,
				CommandResult = new CommandResult
				{
					Status = "ok",
					Body = messageText,
					Bubble = buttons?.ToButtonsOfType<Bubble>() ?? new List<List<Bubble>>(),
					Mentions = mentions
				}
			});
		}

	}
}
