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
				messageText: messageText,
				buttons: null,
				mentions: null
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid[] to, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: syncId,
				to: to,
				messageText: messageText,
				buttons: buttons,
				mentions: null
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				to: null,
				messageText: messageText,
				buttons: null,
				mentions: null
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				to: null,
				messageText: messageText,
				buttons: null,
				mentions: mentions
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageInternalAsync(
				   syncId: requestMessage.SyncId,
				   to: null,
				   messageText: messageText,
				   buttons: buttons,
				   mentions: null
				   );
		}
		private async Task<Guid> ReplyTextMessageInternalAsync(Guid syncId, string messageText, Guid[] to, MessageButtonsGrid buttons, Mention[] mentions)
		{
			return await httpClient.SendReplyAsync(new ResponseMessage
			{
				SyncId = syncId,
				Recipients = to,
				CommandResult = new CommandResult
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
