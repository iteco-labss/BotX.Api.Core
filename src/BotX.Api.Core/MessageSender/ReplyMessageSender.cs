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
				messageSyncId: null,
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
				messageSyncId: null,
				to: to,
				messageText: messageText,
				buttons: buttons,
				mentions: null
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid messageSyncId,  string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: syncId,
				messageSyncId: messageSyncId,
				to: null,
				messageText: messageText,
				buttons: null,
				mentions: null
			);
		}

		public async Task<Guid> ReplyTextMessageAsync(Guid syncId, Guid messageSyncId, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: syncId,
				messageSyncId: messageSyncId,
				to: null,
				messageText: messageText,
				buttons: buttons,
				mentions: null
			);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageSyncId: null,
				to: null,
				messageText: messageText,
				buttons: null,
				mentions: null
			);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, Guid messageSyncId, string messageText)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageSyncId: messageSyncId,
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
				messageSyncId: null,
				to: null,
				messageText: messageText,
				buttons: null,
				mentions: mentions
				);
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, Guid messageSyncId, string messageText, Guid mentionHuid)
		{
			var mentions = new Mention[] { new Mention() { MentionData = new MentionData() { Huid = mentionHuid } } };
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageSyncId: messageSyncId,
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
				   messageSyncId: null,
				   to: null,
				   messageText: messageText,
				   buttons: buttons,
				   mentions: null
				   );
		}

		public async Task<Guid> ReplyTextMessageAsync(UserMessage requestMessage, Guid messageSyncId, string messageText, MessageButtonsGrid buttons)
		{
			return await ReplyTextMessageInternalAsync(
				syncId: requestMessage.SyncId,
				messageSyncId: messageSyncId,
				to: null,
				messageText: messageText,
				buttons: buttons,
				mentions: null
			);
		}

		private async Task<Guid> ReplyTextMessageInternalAsync(Guid syncId, string messageText, Guid? messageSyncId, Guid[] to, MessageButtonsGrid buttons, Mention[] mentions)
		{
			return await httpClient.SendReplyAsync(new ResponseMessage
			{
				SyncId = syncId,
				Recipients = to,
				MessageSyncId = messageSyncId,
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
