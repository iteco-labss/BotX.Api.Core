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
			var row = buttons.AddRow();
			row.AddButton("click me!", testClick, "first");
			row.AddButton("push me!", testClick, "second");
			await MessageSender.SendTextMessageAsync(userMessage, $"You said: {userMessage.Command.Body}", buttons);
		}

		[BotButtonEvent("testClick")]
		private async Task testClick(UserMessage userMessage, string[] args)
		{
			var btn = args.Length > 0 ? args[0] : string.Empty;
			await MessageSender.SendTextMessageAsync(userMessage, $"кнопка нажата {btn}");
		}
	}
}
