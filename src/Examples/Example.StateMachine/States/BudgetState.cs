using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine.States
{
	public class BudgetState : BaseQuestionState
	{
		public override Task ExecuteAsync()
		{
			Model.budget = StateMachine.UserMessage.Command.Body;
			StateMachine.Finish();
			return Task.CompletedTask;
		}

		public override async Task WelcomeAsync()
		{
			await StateMachine.MessageSender.ReplyTextMessageAsync(
				StateMachine.UserMessage,
				"What is your holiday shopping budget this year? Please type it");
		}
	}
}
