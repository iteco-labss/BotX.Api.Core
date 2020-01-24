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
		internal MessageButtonsGridType GridType { get; }

		/// <summary>
		/// Определяет сетку кнопок в сообщении
		/// </summary>
		public MessageButtonsGrid()
		{
			this.GridType = MessageButtonsGridType.Bubble;
		}

		/// <summary>
		/// Определяет сетку кнопок указанного типа
		/// </summary>
		/// <param name="gridType">Тип создаваемых кнопок</param>
		public MessageButtonsGrid(MessageButtonsGridType gridType)
		{
			this.GridType = gridType;
		}

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

		internal IEnumerable<IEnumerable<T>> ToButtonsOfType<T>() where T: BaseButton, new()
		{
			return Rows.Select(
				x => x.Buttons.Select(
					btn => btn != null ?
					new T
					{
						Command = btn.InternalCommand,
						Label = btn.Title,
						Data = btn.Data,
						Options = new BubbleOptions() { Silent = btn.IsSilent }
					} : new T { Label = " ✖️ ", Command = string.Empty }));
		}
	}
}
