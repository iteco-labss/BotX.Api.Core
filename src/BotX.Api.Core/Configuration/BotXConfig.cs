using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
		public HashSet<BotEntry> BotEntries { get; } = new HashSet<BotEntry>();

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
		public BotXConfig(IEnumerable<BotEntry> entries)
		{
			foreach (var e in entries)
				BotEntries.Add(e);
		}

		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="cts">Адрес сервера cts</param>
		/// <param name="botId">Идентификатор бота <c>свойство ID в настройках бота на cts</c></param>
		/// <param name="botSecret">Секретный ключ бота <c>свойство Secret key в настройках бота на cts</c></param>
		public BotXConfig(Uri cts, Guid botId, string botSecret) => BotEntries.Add(new BotEntry(cts, botId, botSecret));

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
			foreach (var e in entries)
				BotEntries.Add(e);
		}

		/// <summary>
		/// Создаёт конфигурацию с одним ботом
		/// </summary>
		/// <param name="cts">Адрес сервера cts</param>
		/// <param name="botId">Идентификатор бота <c>свойство ID в настройках бота на cts</c></param>
		/// <param name="botSecret">Секретный ключ бота <c>свойство Secret key в настройках бота на cts</c></param>
		/// <param name="inChatExceptions">Указывает на необходимость вывода ошибок в пользовательский чат</param>
		public BotXConfig(Uri cts, Guid botId, string botSecret, bool inChatExceptions)
		{
			InChatExceptions = inChatExceptions;
			BotEntries.Add(new BotEntry(cts, botId, botSecret));
		}

		internal BotEntry GetEntryBy(Guid botId)
		{
			var entry = BotEntries.SingleOrDefault(x => x.BotId == botId);
			if (entry == null)
				throw new Exception($"The configuration does not contain BotEntry for bot with id {botId}. Please add cts, botId, secret to your startup (services.AddExpressBot())");

			return entry;
		}
	}
}
