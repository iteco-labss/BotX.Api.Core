using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.Delegates;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot
{
	[BotAction]
	public class ChatMessageAction : BotAction
	{
		public ChatMessageAction(IBotMessageSender messageSender) : base(messageSender)
		{
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			var syncId = Guid.NewGuid();
			var buttons = new MessageButtonsGrid();
			buttons.AddRow().AddSilentButton("Edit message", "edit", new CountClickPayload(syncId));
			buttons.AddRow().AddSilentButton("Simple button handler", "nullArgs");
			buttons.AddRow().AddButton("Button as message");

			await MessageSender.ReplyTextMessageAsync(userMessage, syncId, $"You said: {userMessage.Command.Body}", buttons);
		}

		[BotButtonEvent("edit")]
		private async Task Edit(UserMessage userMessage, CountClickPayload payload)
		{
			payload.Increment();
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddSilentButton("Increment", "edit", payload);
			await MessageSender.EditMessageAsync(userMessage, payload.SyncId, $"Button pressed {payload.Count} times", buttons);
		}

		[BotButtonEvent("nullArgs")]
		private async Task NullArgsClick(UserMessage userMessage, Payload payload)
		{
			await MessageSender.ReplyTextMessageAsync(userMessage, $"Button pressed without arguments");
		}
	}

	public class CountClickPayload : Payload
	{
		public int Count { get; set; }
		public void Increment() => Count++;
		public Guid SyncId { get; }

		public CountClickPayload(Guid syncId)
		{
			SyncId = syncId;
		}
	}
}
