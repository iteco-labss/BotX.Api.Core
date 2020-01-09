using BotX.Api.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BotX.Api.BotUI
{
	/// <summary>
	/// Кнопка, добавляемая в сообщение бота
	/// </summary>
	public class MessageButton
	{
		/// <summary>
		/// Текст на кнопке
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Команда, которая будет отправлена в чат, при нажатии этой кнопки
		/// </summary>
		public string Command { get; }

		internal string[] Arguments { get; set; }

		internal string InternalCommand { get; }

		/// <summary>
		/// Событие, которое будет вызвано при нажатии на кнопку (либо получении команды из свойства Command)
		/// Указанное событие должно быть предварительно зарегистрировано
		/// </summary>
		public BotEventHandler Event { get; }

		/// <summary>
		/// Создаёт кнопку, привязанную к действию
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="event">Событие, выполняемое при нажатии</param>
		/// <param name="args"></param>
		internal MessageButton(string title, BotEventHandler @event, string[] args)
		{
			Title = title;
			Event = @event;
			Arguments = args;

			if (@event == null)
			{
				InternalCommand = title;
			}
			else
			{
				var pair = ActionExecutor.actionEvents.SingleOrDefault(x => x.Value == @event.GetMethodInfo());

				if (!pair.Equals(default(KeyValuePair<string, MethodInfo>)))
					InternalCommand = $"/{pair.Key}";

				if (args != null && args.Length > 0)
					InternalCommand += $" {string.Join(" ", args)}";
			}
		}
	}
}
