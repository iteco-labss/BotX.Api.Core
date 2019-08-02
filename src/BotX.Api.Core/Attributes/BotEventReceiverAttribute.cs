using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.Attributes
{
	/// <summary>
	/// Указывает, что данный класс будет содержать обработчики событий (например обработчики нажатий кнопок)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class BotEventReceiverAttribute : Attribute
	{
	}
}
