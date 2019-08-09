using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotX.Api.JsonModel.Request
{
#pragma warning disable CS1591
	public class UserMessage
	{
		[JsonProperty("sync_id")]
		public Guid SyncId { get; set; }

		[JsonProperty("command")]
		public Command Command { get; set; }

		[JsonProperty("file")]
		public File File { get; set; }

		[JsonProperty("from")]
		public From From { get; set; }

		[JsonProperty("bot_id")]
		public Guid BotId { get; set; }
	}
}
