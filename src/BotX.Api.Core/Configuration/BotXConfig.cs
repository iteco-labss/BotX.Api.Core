using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BotX.Api.Configuration;

namespace BotX.Api
{
	/// <summary>
	/// Конфигурирует поведение BotX
	/// </summary>
	public class BotXConfig
	{
		[Required]
		public List<BotEntry> BotEntries { get; set; } = new List<BotEntry>();

		/// <summary>
		/// Указывает, нужно ли выводить сообщения об ошибках в чат
		/// </summary>
		public bool InChatExceptions { get; set; } = false;

		public BotXConfig(BotEntry entry) => BotEntries.Add(entry);

		public BotXConfig(IEnumerable<BotEntry> entries) => BotEntries.AddRange(entries);

		public BotXConfig(Guid botId, string botSecret) => BotEntries.Add(new BotEntry(botId, botSecret));

		public BotXConfig(BotEntry entry, bool inChatExceptions)
		{
			BotEntries.Add(entry);
			InChatExceptions = inChatExceptions;
		}

		public BotXConfig(IEnumerable<BotEntry> entries, bool inChatExceptions)
		{
			InChatExceptions = inChatExceptions;
			BotEntries.AddRange(entries);
		}

		public BotXConfig(Guid botId, string botSecret, bool inChatExceptions)
		{
			InChatExceptions = inChatExceptions;
			BotEntries.Add(new BotEntry(botId, botSecret));
		}
	}
}
