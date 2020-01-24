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
		/// Идентификатор бота, служит для авторизации на cts.
		/// </summary>
		[Required]
		public Guid BotId { get; set; }
		/// <summary>
		/// Секретный ключ, служит для авторизации на cts.
		/// </summary>
		[Required]
		public string SecretKey { get; set; }
		/// <summary>
		/// Нужно ли выводить сообщения об ошибках в чат
		/// </summary>
		public bool InChatExceptions { get; set; } = false;
	}
}
