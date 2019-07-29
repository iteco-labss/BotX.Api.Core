using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotX.Api.JsonModel.Response
{
	public class File
	{
		[JsonProperty("data")]
		public string Data { get; set; }
		[JsonProperty("file_name")]
		public string FileName { get; set; }
	}
}
