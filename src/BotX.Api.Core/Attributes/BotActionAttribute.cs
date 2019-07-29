using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BotX.Api.Attributes
{
	/// <summary>
	/// Атрибут применяемый к классу-контроллеру IBotAction, для указания команды, на которую будет реагировать контроллер
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BotActionAttribute : Attribute
	{
		/// <summary>
		/// Указывает на какую команду должен реагировать данный контроллер
		/// </summary>
		/// <param name="action">Команда, например start. Недопускается ввод специальных символов и пробелов</param>
		public BotActionAttribute(string action)
		{
			if (Regex.IsMatch(action, @"[^a-zA-Z0-9\-\\_]"))
				throw new ArgumentException("The action name should contains only letters, numbers, _ or -");

			Action = action.ToLower();
		}

		internal string Action { get; }
	}
}
