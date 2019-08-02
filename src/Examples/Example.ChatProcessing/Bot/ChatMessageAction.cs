using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot
{
	[BotAction]
	public class ChatMessageAction : BotAction
	{
		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
			var buttons = new MessageButtonsGrid();
			buttons.AddRow().AddButton("test", testClick);
			await MessageSender.SendTextMessageAsync(userMessage, $"You said: {userMessage.Command.Body}", buttons);
		}

		[BotButtonEvent("testClick")]
		private async Task testClick(UserMessage userMessage, string[] args)
		{
			await MessageSender.SendTextMessageAsync(userMessage, "кнопка нажата");
		}
	}
}
