using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class ResponseMessage
    {
		[JsonProperty(PropertyName = "sync_id")]
		public Guid SyncId { get; set; }

		[JsonProperty(PropertyName = "recipients", NullValueHandling =NullValueHandling.Ignore)]
		public Guid[] Recipients { get; set; }

		[JsonProperty(PropertyName = "command_result")]
		public CommandResult CommandResult { get; set; }

	}
}
