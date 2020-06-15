using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.Configuration
{
	public class BotEntry
	{
		public Guid BotId { get; }
		public string Secret { get; }

		public BotEntry(Guid botId, string secret)
		{
			BotId = botId;
			Secret = secret;
		}
	}
}
