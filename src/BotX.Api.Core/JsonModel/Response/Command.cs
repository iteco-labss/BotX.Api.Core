using BotX.Api.JsonModel.Menu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
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
	}




}
