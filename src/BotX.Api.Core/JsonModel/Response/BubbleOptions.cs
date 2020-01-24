using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class BubbleOptions
	{
		[JsonProperty("silent")]
		public bool Silent { get; set; }
	}
}
