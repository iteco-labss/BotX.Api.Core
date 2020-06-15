using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using Microsoft.Extensions.DependencyInjection;
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
		private readonly DemoStateMachine stateMachine;
		public StartAction(IBotMessageSender messageSender, DemoStateMachine stateMachine) : base(messageSender)
		{
			this.stateMachine = stateMachine;
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			await stateMachine.EnterAsync(userMessage);
		}
	}
}
