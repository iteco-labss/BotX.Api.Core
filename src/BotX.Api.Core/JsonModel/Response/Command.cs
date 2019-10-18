using BotX.Api.JsonModel.Menu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class Command
	{
		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("body")]
		public string Body { get; set; }

		[JsonProperty("options")]
		public Option Options { get; set; }

		[JsonProperty("elements")]
		public Element[] Elements { get; set; } = new Element[0];

		[JsonProperty("command_type")]
		public string Type { get; set; }

		[JsonProperty("data")]
		public Data Data { get; set; }
	}




}
