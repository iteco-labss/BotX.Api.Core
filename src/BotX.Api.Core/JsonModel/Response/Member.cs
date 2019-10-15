using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	public class Member
	{
		[JsonProperty("huid")]
		public Guid Huid { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("user_kind")]
		public string UserKind { get; set; }

		[JsonProperty("admin")]
		public bool IsAdmin { get; set; }
	}
}
