using BotX.Api;
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
		private readonly BotMessageSender sender;

		public SayHelloAction(BotMessageSender sender)
		{
			this.sender = sender;
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			await sender.SendTextMessageAsync(userMessage, "Hello. How are you?");
		}
	}
}
