using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class Bubble
	{
		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("command")]
		public string Command { get; set; }
	}
}
