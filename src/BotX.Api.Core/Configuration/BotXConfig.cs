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
		/// <summary>
		/// Записи о всех ботах, с которыми может работать данное приложение
		/// </summary>
		public List<BotEntry> BotEntries { get; } = new List<BotEntry>();

		/// <summary>
		/// Указывает, нужно ли выводить сообщения об ошибках в чат
		/// </summary>
		public bool InChatExceptions { get; } = false;
		
		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="entry">Информация о боте</param>
		public BotXConfig(BotEntry entry) => BotEntries.Add(entry);

		/// <summary>
		/// Создаёт конфигурацию с множеством ботов
		/// </summary>
		/// <param name="entries">Информация о ботах</param>
		public BotXConfig(IEnumerable<BotEntry> entries) => BotEntries.AddRange(entries);

		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="botId">Идентификатор бота <c>свойство ID в настройках бота на cts</c></param>
		/// <param name="botSecret">Секретный ключ бота <c>свойство Secret key в настройках бота на cts</c></param>
		public BotXConfig(Guid botId, string botSecret) => BotEntries.Add(new BotEntry(botId, botSecret));

		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="entry">Информация о боте</param>
		/// <param name="inChatExceptions">Указывает на необходимость вывода ошибок в пользовательский чат</param>
		public BotXConfig(BotEntry entry, bool inChatExceptions)
		{
			BotEntries.Add(entry);
			InChatExceptions = inChatExceptions;
		}

		/// <summary>
		/// Создаёт конфигурацию с множеством ботов
		/// </summary>
		/// <param name="entries">Информация о ботах</param>
		/// <param name="inChatExceptions">Указывает на необходимость вывода ошибок в пользовательский чат</param>
		public BotXConfig(IEnumerable<BotEntry> entries, bool inChatExceptions)
		{
			InChatExceptions = inChatExceptions;
			BotEntries.AddRange(entries);
		}

		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="botId">Идентификатор бота <c>свойство ID в настройках бота на cts</c></param>
		/// <param name="botSecret">Секретный ключ бота <c>свойство Secret key в настройках бота на cts</c></param>
		/// <param name="inChatExceptions">Указывает на необходимость вывода ошибок в пользовательский чат</param>
		public BotXConfig(Guid botId, string botSecret, bool inChatExceptions)
		{
			InChatExceptions = inChatExceptions;
			BotEntries.Add(new BotEntry(botId, botSecret));
		}
	}
}
