using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.Delegates;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <param name="command">Текст отправляемый в чат, при нажатии кнопки</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <param name="payload">Дополнитеьные аргументы, которые будут переданны в метод-обработчик</param>
		/// <returns></returns>
		public MessageButton AddButton<T>(string title, string command, BotEventHandler<T> handler, T payload) where T : Payload
		{
			var btn = MessageButton.Create(
				title: title,
				command: command,
				@event: handler,
				payload: payload,
				isSilent: false);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создаёт кнопку внутри данной строки
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <param name="payload">Дополнитеьные аргументы, которые будут переданны в метод-обработчик</param>
		/// <returns></returns>
		public MessageButton AddButton<T>(string title, BotEventHandler<T> handler, T payload) where T : Payload
		{
			var btn = MessageButton.Create(
				title: title,
				command: null,
				@event: handler,
				payload: payload,
				isSilent: false);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создаёт кнопку внутри данной строки
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <returns></returns>
		public MessageButton AddButton(string title, BotEventHandler<Payload> handler)
		{
			var btn = MessageButton.Create(
				title: title,
				command: null,
				@event: handler,
				payload: null,
				isSilent: false);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создает кнопку, нажатие на которую не генирирует сообщение в чат
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <param name="payload">Дополнитеьные аргументы, которые будут переданны в метод-обработчик</param>
		/// <returns></returns>
		public MessageButton AddSilentButton<T>(string title, BotEventHandler<T> handler, T payload) where T : Payload
		{
			var btn = MessageButton.Create(
				title: title,
				command: null,
				@event: handler,
				payload: payload,
				isSilent: true);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создает кнопку, нажатие на которую не генирирует сообщение в чат
		/// </summary>
		/// <param name="title">Текст на кнопке</param>
		/// <param name="handler">Метод-обработчик нажатия на кнопку</param>
		/// <returns></returns>
		public MessageButton AddSilentButton(string title, BotEventHandler<Payload> handler)
		{
			var btn = MessageButton.Create(
				title: title,
				command: null,
				@event: handler,
				payload: null,
				isSilent: true);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создаёт кнопку <c>без обработчика</c> внутри данной строки. Нажатие на эту кнопку генерирует сообщение с названием кнопки
		/// </summary>
		/// <param name="title">Название кнопки</param>
		/// <returns></returns>
		public MessageButton AddButton(string title)
		{
			var btn = MessageButton.Create<Payload>(
				title: title,
				command: null,
				@event: null,
				payload: null,
				isSilent: false);

			Buttons.Add(btn);
			return btn;
		}

		/// <summary>
		/// Создаёт пустую ячейку вместо кнопки
		/// </summary>
		public void AddEmpty()
		{
			Buttons.Add(null);
		}
	}
}
