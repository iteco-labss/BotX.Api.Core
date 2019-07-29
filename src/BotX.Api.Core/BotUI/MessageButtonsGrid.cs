using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.BotUI
{
	/// <summary>
	/// Представляет сетку кнопок, встраивыемых в сообщение от бота
	/// </summary>
	public class MessageButtonsGrid
	{
		/// <summary>
		/// Строки с кнопками
		/// </summary>
		public List<MessageButtonsRow> Rows { get; } = new List<MessageButtonsRow>();

		/// <summary>
		/// Добавляет новую строку к уже имеющимся
		/// </summary>
		/// <returns></returns>
		public MessageButtonsRow AddRow()
		{
			var row = new MessageButtonsRow();
			Rows.Add(row);
			return row;
		}
	}
}
