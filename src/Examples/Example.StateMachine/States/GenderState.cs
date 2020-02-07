using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine.States
{
	public class GenderState : BaseQuestionState
	{
		public override async Task ExecuteAsync()
		{

		}

		public override async Task WelcomeAsync()
		{
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddSilentButton("Male", "GenderSelect", new GenderPayload() { Gender = "Male" });
			row.AddSilentButton("Female", "GenderSelect", new GenderPayload() { Gender = "Female" });
			await StateMachine.MessageSender.ReplyTextMessageAsync(StateMachine.UserMessage, "Are you...?", buttons);
		}
	}


}
