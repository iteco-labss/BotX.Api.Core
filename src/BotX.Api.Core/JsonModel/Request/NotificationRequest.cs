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

		[JsonProperty(PropertyName = "group_chat_ids", NullValueHandling = NullValueHandling.Ignore)]
		public Guid[] GroupChatIds { get; set; }

		[JsonProperty("notification")]
		public CommandResult Notification { get; set; }

	}
}
