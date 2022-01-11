using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.BotUI;
using BotX.Api.Delegates;
using BotX.Api.JsonModel.Api.Request;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot
{
	[BotAction]
	public class ChatMessageAction : BotAction
	{
		public ChatMessageAction(IBotMessageSender messageSender) : base(messageSender)
		{
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
		{
            if (userMessage.Attachments.Any())
            {
                await UploadFile(userMessage);
				return;
            }

            var payload = new CountClickPayload(Guid.NewGuid()); // фейковый id, т.к. id сообщения не известен до отправки самого сообщения

			var buttons = new MessageButtonsGrid();
            buttons.AddRow().AddSilentButton("Edit message", "edit", payload);
            buttons.AddRow().AddSilentButton("Simple button handler", "nullArgs");
            buttons.AddRow().AddButton("Button as message");
            buttons.AddRow().AddSilentButton("Get my file!", "fileRequest");

			var syncId = await MessageSender.ReplyTextMessageAsync(userMessage, $"You said: {userMessage.Command.Body}", buttons);
            payload.SyncId = syncId;

            buttons.Rows.First().Buttons.First().ChangePayload(payload);

			await MessageSender.EditMessageAsync(userMessage, syncId, $"You said: {userMessage.Command.Body}", buttons);
		}

		[BotButtonEvent("edit")]
		private async Task Edit(UserMessage userMessage, CountClickPayload payload)
		{
			payload.Increment();
			var buttons = new MessageButtonsGrid();
			var row = buttons.AddRow();
			row.AddSilentButton("Increment", "edit", payload);
			await MessageSender.EditMessageAsync(userMessage, payload.SyncId, $"Button pressed {payload.Count} times", buttons);
		}

		[BotButtonEvent("nullArgs")]
		private async Task NullArgsClick(UserMessage userMessage, Payload payload)
		{
			await MessageSender.ReplyTextMessageAsync(userMessage, $"Button pressed without any arguments");
		}

		[BotButtonEvent("fileRequest")]
		private async Task GetFileClick(UserMessage userMessage, Payload payload)
		{
			await MessageSender.SendFileAsync(userMessage, "simple_file.txt", System.Text.Encoding.UTF8.GetBytes("Yes, I'm Attachments!"));
		}

		private async Task UploadFile(UserMessage userMessage)
		{
			var attachment = userMessage.Attachments.First();
			var data = System.Text.Encoding.UTF8.GetBytes(attachment.Data.Content ?? "");
			var response = await MessageSender.UploadFileAsync(userMessage, attachment.Data.FileName ?? "", data, new FileMetaInfo { Duration = attachment.Data.Duration });
			await MessageSender.ReplyTextMessageAsync(userMessage, $"Attachments info:\r\n{JsonConvert.SerializeObject(response.Result, Formatting.Indented)}");
		}
	}

	public class CountClickPayload : Payload
	{
		public int Count { get; set; }
		public void Increment() => Count++;
		public Guid SyncId { get; set; }

		public CountClickPayload(Guid syncId)
		{
			SyncId = syncId;
		}
	}
}