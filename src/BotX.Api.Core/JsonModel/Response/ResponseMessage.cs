﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class ResponseMessage
	{
		[JsonProperty(PropertyName = "sync_id")]
		public Guid SyncId { get; set; }

		[JsonProperty(PropertyName = "to")]
		public Guid Recipient { get; set; }

		[JsonProperty(PropertyName = "bot_id")]
		public Guid BotId { get; set; }

		[JsonProperty(PropertyName = "command_result")]
		public CommandResult CommandResult { get; set; }
	}
}
