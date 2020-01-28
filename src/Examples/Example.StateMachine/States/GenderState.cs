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
			row.AddSilentButton("Male", NullArgsClick, new GenderPayload() { Gender = "Male" });
			row.AddSilentButton("Female", NullArgsClick, new GenderPayload() { Gender = "Female" });
			await StateMachine.MessageSender.ReplyTextMessageAsync(StateMachine.UserMessage, "Are you...?", buttons);
		}

		[BotButtonEvent("GenderSelect")]
		private async Task NullArgsClick(UserMessage userMessage, GenderPayload payload)
		{
			var data = payload;
			Model.gender = data.Gender;
			await StateMachine.TransitionToAsync<RecommendState>();
		}
	}

	class GenderPayload : Payload
	{
		public string Gender { get; set; }
	}
}
