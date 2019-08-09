using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BotX.Api.Attributes
{
	/// <summary>
	/// Атрибут, применяемый к методу-событию внутри класса контроллера IBotAction, 
	/// который будет вызываться при выполнении действий внутри данного класса (например нажатие на кнопки)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class BotButtonEventAttribute : Attribute
	{
		/// <summary>
		/// Указывает, что данный метод является обработчиком события действия бота
		/// </summary>
		/// <param name="eventName">Имя события, на которое будет реагировать данное событие</param>
		public BotButtonEventAttribute(string eventName)
		{
			if (Regex.IsMatch(eventName, @"[^a-zA-Z0-9\-\\_]"))
				throw new ArgumentException("The BotActioveEvent name should contains only letters, numbers, _ or -");

			EventName = eventName.ToLower();
		}

		internal string EventName { get; }
	}
}
