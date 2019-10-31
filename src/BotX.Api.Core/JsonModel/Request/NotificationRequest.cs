using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Request
{
#pragma warning disable CS1591
	public class NotificationMessage : IMessage
	{
		[JsonProperty("bot_id")]
		public Guid BotId { get; set; }

		[JsonProperty("recipients")]
		public Guid[] Recipients { get; set; }

		[JsonProperty("group_chat_ids")]
		public Guid[] GroupChatIds { get; set; }

		[JsonProperty("notification")]
		public CommandResult Notification { get; set; }
	}
}
