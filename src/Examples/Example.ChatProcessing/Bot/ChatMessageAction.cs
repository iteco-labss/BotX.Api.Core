using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
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
			await MessageSender.SendTextMessageAsync(userMessage, $"кнопка нажата {args[0]}");
		}

		public override async Task OnChatCreated(UserMessage userMessage)
		{
			await MessageSender.SendTextMessageAsync(userMessage, "hi");
			/*if (data.GroupChatId.HasValue && data.Creator.HasValue && data.Members.Length > 0)
				await MessageSender.SendTextMessageAsync(
					new Guid[] { data.GroupChatId.Value },
					new Guid[] { data.Creator.Value },
					$"Привет {data.Members.First().Name}, ты создал чат {data.Name}");*/
		}
	}
}
