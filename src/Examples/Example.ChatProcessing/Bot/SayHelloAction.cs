﻿using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot
{
	[BotAction("sayhello")]
	public class SayHelloAction : BotAction
	{
		public SayHelloAction(IBotMessageSender messageSender) : base(messageSender)
		{
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			await MessageSender.ReplyTextMessageAsync(userMessage, "Hello. How are you?", userMessage.From.Huid);
		}
	}
}
