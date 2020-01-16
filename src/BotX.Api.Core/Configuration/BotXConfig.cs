using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BotX.Api
{
	/// <summary>
	/// Конфигурирует поведение BotX
	/// </summary>
	public class BotXConfig
	{
		/// <summary>
		/// Адрес сервиса cts. Например https://cts.example.com
		/// </summary>
		[Required]
		public string CtsServiceUrl { get; set; }
		/// <summary>
		/// Идентификатор бота, если BotId не указанн то бот поддерживает только ответ на входящие сообщения от пользователя
		/// </summary>
		public Guid BotId { get; set; } = Guid.Empty;
		/// <summary>
		/// Секретный ключ, служит для авторизации на cts.(необходим для 3+ версии АПИ) 
		/// </summary>
		public string SecretKey { get; set; } = null;
		/// <summary>
		/// Нужно ли выводить сообщения об ошибках в чат
		/// </summary>
		public bool InChatExceptions { get; set; } = false;
	}
}
