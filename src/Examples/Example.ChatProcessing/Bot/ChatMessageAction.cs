﻿using BotX.Api;
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
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddButton("click me!", CountClick, new CountClickPayload());
			row.AddButton("push me!", NullArgsClick, null);

			await MessageSender.ReplyTextMessageAsync(userMessage, $"You said: {userMessage.Command.Body}", buttons);
			
		}

		[BotButtonEvent("count")]
		private async Task CountClick(UserMessage userMessage, Payload payload)
		{
			var data = (CountClickPayload)payload;
			await MessageSender.ReplyTextMessageAsync(userMessage, $"Button pressed {data.Count}");
			data.Increment();
		}

		[BotButtonEvent("nullArgs")]
		private async Task NullArgsClick(UserMessage userMessage, Payload payload)
		{
			await MessageSender.ReplyTextMessageAsync(userMessage, $"Button pressed without arguments");
		}
	}

	public class CountClickPayload : Payload
	{
		private static int count = 0;

		public int Count => count;
		public void Increment() => Interlocked.Increment(ref count);
	}
}
