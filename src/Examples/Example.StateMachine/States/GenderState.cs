using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine.States
{
	public class GenderState : BaseQuestionState
	{
		public override async Task ExecuteAsync(dynamic model)
		{
			model.gender = StateMachine.UserMessage.Command.Body;
			await StateMachine.TransitionToAsync<RecommendState>();
		}

		public override async Task WelcomeAsync(dynamic model)
		{
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddButton("Male");
			row.AddButton("Female");
			await StateMachine.MessageSender.ReplyTextMessageAsync(StateMachine.UserMessage, "Are you...?", buttons);
		}
	}
}
