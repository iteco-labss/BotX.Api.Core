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
			var fakeSyncId = Guid.NewGuid();
			var fakeButtons = new MessageButtonsGrid();
			fakeButtons.AddRow().AddSilentButton("Edit message", "edit", new CountClickPayload(fakeSyncId));
			fakeButtons.AddRow().AddSilentButton("Simple button handler", "nullArgs");
			fakeButtons.AddRow().AddButton("Button as message");
			fakeButtons.AddRow().AddSilentButton("Get my file!", "fileRequest");
			fakeButtons.AddRow().AddSilentButton("Upload my file!", "uploadFileRequest");

			var syncId = await MessageSender.ReplyTextMessageAsync(userMessage, $"You said: {userMessage.Command.Body}", fakeButtons);

			var buttons = new MessageButtonsGrid();
			buttons.AddRow().AddSilentButton("Edit message", "edit", new CountClickPayload(syncId));
			buttons.AddRow().AddSilentButton("Simple button handler", "nullArgs");
			buttons.AddRow().AddButton("Button as message");
			buttons.AddRow().AddSilentButton("Get my file!", "fileRequest");
			buttons.AddRow().AddSilentButton("Upload my file!", "uploadFileRequest");

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
			await MessageSender.SendFileAsync(userMessage, "simple_file.txt", System.Text.Encoding.UTF8.GetBytes("Yes, I'm File!"));
		}

		[BotButtonEvent("uploadFileRequest")]
		private async Task UploadFileClick(UserMessage userMessage, Payload payload)
		{
			var fileName = "simple_file.txt";
			var data = System.Text.Encoding.UTF8.GetBytes("Yes, I'm File!");
			var response = await MessageSender.UploadFileAsync(userMessage, fileName, data, new FileMetaInfo { Caption = "test caption" });
			await MessageSender.ReplyTextMessageAsync(userMessage, $"File info:\r\n{JsonConvert.SerializeObject(response, Formatting.Indented)}");
		}
	}

	public class CountClickPayload : Payload
	{
		public int Count { get; set; }
		public void Increment() => Count++;
		public Guid SyncId { get; }

		public CountClickPayload(Guid syncId)
		{
			SyncId = syncId;
		}
	}
}
