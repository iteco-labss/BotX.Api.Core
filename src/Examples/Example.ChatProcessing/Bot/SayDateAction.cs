using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot
{
	[BotAction("saydate")]
	public class SayDateAction : BotAction
	{
		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			await MessageSender.ResponseTextMessageAsync(userMessage, DateTime.Now.ToShortDateString());
		}
	}
}
