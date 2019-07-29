using Newtonsoft.Json;
using System.Collections.Generic;

namespace BotX.Api.JsonModel.Response
{
	public class CommandResult
	{
		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("body")]
		public string Body { get; set; }

		[JsonProperty("commands")]
		public Command[] Commands { get; set; } = new Command[0];

		[JsonProperty("bubble")]
		public IEnumerable<IEnumerable<Bubble>> Bubble { get; set; } = new List<List<Bubble>>();

		[JsonProperty("keyboard")]
		public object[] Keyboard { get; set; } = new object[0];

		[JsonProperty("files")]
		public File[] Files { get; set; } = new File[0];
	}
}