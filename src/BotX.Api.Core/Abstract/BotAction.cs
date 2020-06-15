using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotX.Api.JsonModel.Request;
using Microsoft.Extensions.Logging;

namespace BotX.Api.Abstract
{
	/// <summary>
	/// Класс контроллера действия бота. 
	/// Используйте атрибут BotAction для указания команды, на которую должен реагировать данный контроллер.
	/// </summary>
	public abstract class BotAction : IBotAction
	{
		/// <summary>
		/// Отправщик сообщений через мессенджер
		/// </summary>
		protected IBotMessageSender MessageSender { get; private set; }

		/// <summary>
		/// Сериализованное сообщение от пользователя
		/// </summary>
		protected UserMessage RequestMessage { get; private set; }

#pragma warning disable CS1591 
		public BotAction(IBotMessageSender messageSender)
#pragma warning restore CS1591
		{
			MessageSender = messageSender;
		}

		async Task IBotAction.InternalExecuteAsync(UserMessage userMessage, string[] args)
		{
			RequestMessage = userMessage;
			await ExecuteAsync(userMessage, args);
		}

		/// <summary>
		/// Метод, вызываемый при получении сообщения от пользователя
		/// </summary>
		/// <param name="userMessage">Пользовательское сообщение</param>
		/// <param name="args">Дополнительные аргументы сообщения</param>
		/// <returns></returns>
		public abstract Task ExecuteAsync(UserMessage userMessage, string[] args);
	}
}
