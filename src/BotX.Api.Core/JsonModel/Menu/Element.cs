using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Menu
{
	public class Element
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("order")]
		public int Order { get; set; }
	}
}
