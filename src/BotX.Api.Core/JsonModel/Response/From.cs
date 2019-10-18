using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class From
	{
		[JsonProperty("user_huid")]
		public Guid? Huid { get; set; }

		[JsonProperty("group_chat_id")]
		public Guid? ChatId { get; set; }

		[JsonProperty("chat_type")]
		public string ChatType { get; set; }

		[JsonProperty("ad_login")]
		public string Login { get; set; }

		[JsonProperty("ad_domain")]
		public string Domain { get; set; }

		[JsonProperty("username")]
		public string UserName { get; set; }

		[JsonProperty("host")]
		public string Host { get; set; }
	}
}
