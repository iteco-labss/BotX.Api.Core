using BotX.Api.BotUI;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine.States
{
	public class RecommendState : BaseQuestionState
	{
		public override async Task ExecuteAsync()
		{
			Model.recommend = StateMachine.UserMessage.Command.Body;
			await StateMachine.TransitionToAsync<BudgetState>();
		}

		public override async Task WelcomeAsync()
		{
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();

			for (int i = 1; i <= 5; i++)
				row.AddButton(i.ToString());

			await StateMachine.MessageSender.ReplyTextMessageAsync(
				StateMachine.UserMessage,
				"How likely is it that you would recommend this " +
				"bot to a friend or colleague?",
				buttons);
		}
	}
}
