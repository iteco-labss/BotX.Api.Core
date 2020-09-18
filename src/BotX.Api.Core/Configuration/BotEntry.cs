using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BotX.Api.Configuration
{
	/// <summary>
	/// Запись о боте
	/// </summary>
	public class BotEntry
	{
		/// <summary>
		/// Идентификатор бота
		/// <para><c>свойство ID в настройках бота на cts</c></para>
		/// </summary>
		public Guid BotId { get; }

		/// <summary>
		/// Секретный ключ бота
		/// <para><c>свойство Secret key в настройках бота на cts</c></para>
		/// </summary>
		public string Secret { get; }

		/// <summary>
		/// Адрес сервера CTS
		/// </summary>
		public Uri Cts { get; }

		/// <summary>
		/// Создаёт экземпляр записи о боте
		/// </summary>
		/// <param name="cts">Адрес сервера CTS</param>
		/// <param name="botId">Идентификатор бота (<c>свойство ID в настройках бота на cts</c>)</param>
		/// <param name="secret">Секретный ключ бота (<c>свойство Secret key в настройках бота на cts</c>)</param>
		public BotEntry(Uri cts, Guid botId, string secret)
		{
			Cts = cts;
			BotId = botId;
			Secret = secret;
		}
	}
}
