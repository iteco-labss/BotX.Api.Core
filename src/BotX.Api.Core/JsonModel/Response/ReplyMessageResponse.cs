using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	class ReplyMessageResponse
	{
		[JsonProperty("status")]
		public string Status { get; set; }
		[JsonProperty("result")]
		public ReplyMessageResponseResult Result { get; set; }
	}

	class ReplyMessageResponseResult
	{
		[JsonProperty("sync_id")]
		public Guid SyncId { get; set; }
	}
}
