using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
	/// <summary>
	/// Базовый класс для передачи данных в события обработки кнопок
	/// </summary>
	public abstract class Payload
	{
		/// <summary>
		/// Идентификатор сообщения в Express (по которому его можно редактировать).
		/// Его можно передать в метод отправки сообщения (параметр messageSyncId)
		/// </summary>
		public Guid? MessageSyncId { get; set; }
	}
}
