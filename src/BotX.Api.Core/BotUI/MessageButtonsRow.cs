﻿using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.Delegates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BotX.Api.BotUI
{
	/// <summary>
	/// Строка, содержащая в себе кнопки, встраиваемые в сообщение от бота
	/// </summary>
	public class MessageButtonsRow
	{
		/// <summary>
		/// Кнопки внутри данной строки
		/// </summary>
		public List<MessageButton> Buttons { get; } = new List<MessageButton>();
		
		/// <summary>
		/// Создаёт кнопку внутри данной строки
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <param name="args">Дополнитеьные аргументы, которые будут возвращены в метод-обработчик</param>
		/// <returns></returns>
		public MessageButton AddButton(string title, BotEventHandler handler, params string[] args)
		{
			var btn = new MessageButton(
				title: title,
				@event: handler,
				args: args);

			Buttons.Add(btn);
			return btn;
		}
	}
}
