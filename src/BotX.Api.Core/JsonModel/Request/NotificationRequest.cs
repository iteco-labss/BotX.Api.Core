using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Request
{
#pragma warning disable CS1591
	public class NotificationMessage
	{
		[JsonProperty(PropertyName = "recipients", NullValueHandling = NullValueHandling.Ignore)]
		public Guid[] Recipients { get; set; }

		[JsonProperty(PropertyName = "group_chat_id", NullValueHandling = NullValueHandling.Ignore)]
		public Guid GroupChatId { get; set; }

		[JsonProperty(PropertyName = "event_sync_id", NullValueHandling = NullValueHandling.Ignore)]
		public Guid? MessageSyncId { get; set; }

		[JsonProperty("notification")]
		public CommandResult Notification { get; set; }

		[JsonProperty("file")]
		public File File { get; set; }
	}
}
