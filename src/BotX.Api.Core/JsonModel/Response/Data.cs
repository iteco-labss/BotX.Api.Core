using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class Data
	{
#pragma warning disable CS1591

		[JsonProperty("group_chat_id")]
		public Guid GroupChatId { get; set; } = Guid.Empty;

		[JsonProperty("chat_type")]
		public string ChatType { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("creator")]
		public Guid Creator { get; set; }

		[JsonProperty("members")]
		public Member[] Members { get; set; }
	}
}
