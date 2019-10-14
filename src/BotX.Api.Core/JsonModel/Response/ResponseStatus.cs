using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class ResponseStatus
	{
		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("result")]
		public ResponseResult Result { get; set; }
	}

	public class ResponseResult
	{
		[JsonProperty("enabled")]
		public bool Enabled { get; set; }

		[JsonProperty("status_message")]
		public string Status_message { get; set; }

		[JsonProperty("commands")]
		public Command[] Commands { get; set; }
	}
}
