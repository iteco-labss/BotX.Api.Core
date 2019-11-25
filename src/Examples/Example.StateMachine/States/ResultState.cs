using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
	[BotEventReceiver]
    public class ResultState : BaseState
    {
        public override async Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
			await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, $"Thanks!");
			var btns = new MessageButtonsGrid();
			var row = btns.AddRow();
			row.AddButton("Create", createClick);
			row.AddButton("Cancel", cancelClick);
			await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, "Please choose", btns);
		}

		[BotButtonEvent("cancel")]
		private async Task cancelClick(UserMessage userMessage, string[] args)
		{
			await StateMachine.TransitionToAsync<EnterNameState>();
			//TODO: сделать работу кнопок в StateMachine
		}

		[BotButtonEvent("create")]
		private Task createClick(UserMessage userMessage, string[] args)
		{
			StateMachine.Finish();
			return Task.CompletedTask;
		}
	}
}
