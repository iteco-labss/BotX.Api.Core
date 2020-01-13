using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.Configuration
{
	public class BotXConfig
	{
		public string CtsServiceUrl { get; set; }
		public Guid BotId { get; set; }
		public string SecretKey { get; set; }
		// TODO Мне кажется не лучшая идея хранить здесь токен и менять конфиг
		public string AuthToken { get; set; }
		public bool inChatExceptions { get; set; }
	}
}
