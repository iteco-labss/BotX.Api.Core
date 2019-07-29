using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Request
{
	public class NotificationRequest
	{
		[JsonProperty("bot_id")]
		public Guid BotId { get; set; }

		[JsonProperty("sync_id")]
		public Guid SyncId { get; set; }

		[JsonProperty("to")]
		public Guid Recipient { get; set; }

		[JsonProperty("group_chat_ids")]
		public Guid[] GroupChatIds { get; set; }

		[JsonProperty("notification")]
		public CommandResult Notification { get; set; }
	}
}
