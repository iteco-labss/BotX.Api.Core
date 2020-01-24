using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class BaseButton
	{
		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("command")]
		public string Command { get; set; }

		[JsonProperty("data")]
		public Data Data { get; set; }

		[JsonProperty("opts")]
		public BubbleOptions Options { get; set; }
	}
}
