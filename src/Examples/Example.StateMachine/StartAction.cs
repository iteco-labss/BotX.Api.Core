using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using Example.ChatProcessing.Bot.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Example.StateMachine
{
	[BotAction("start")]
	public class StartAction : BotAction
	{
		public StartAction(IBotMessageSender messageSender) : base(messageSender)
		{
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			DemoStateMachine dsm = new DemoStateMachine(MessageSender);
			await dsm.EnterAsync(userMessage);
		}
	}
}
