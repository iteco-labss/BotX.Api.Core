using System;
using System.Collections.Generic;
using System.Text;
using BotX.Api.JsonConverters;
using BotX.Api.JsonModel.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotX.Api.JsonModel.Request
{
	[JsonConverter(typeof(EntitieConverter))]
	public class Entitie
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("data")]
		public object Data { get; set; }
	}
}
