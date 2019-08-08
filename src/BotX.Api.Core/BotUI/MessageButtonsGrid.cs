using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
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

		internal IEnumerable<IEnumerable<Bubble>> GetBubbles()
		{
			return Rows.Select(
				x => x.Buttons.Select(
					btn => btn != null ?
					new Bubble
					{
						Command = btn.InternalCommand,
						Label = btn.Title
					} : new Bubble { Label = " ✖️ ", Command = string.Empty }));
		}
	}
}
