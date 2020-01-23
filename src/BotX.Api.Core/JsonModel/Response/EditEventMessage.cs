using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class EditEventMessage
	{
		[JsonProperty("sync_id")]
		public Guid SyncId { get; set; }

		[JsonProperty("payload")]
		public CommandResult Payload { get; set; }
	}
}
