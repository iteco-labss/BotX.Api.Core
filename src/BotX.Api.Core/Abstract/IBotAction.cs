using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Abstract
{
	/// <summary>
	/// Интерфейс контроллера действия бота. Используйте атрибут BotAction для указания команды, на которую должен реагировать данный контроллер.
	/// </summary>
	public interface IBotAction
	{
		/// <summary>
		/// Метод, вызываемый при получении сообщения от пользователя
		/// </summary>
		/// <param name="userMessage">Пользовательское сообщение</param>
		/// <param name="args">Дополнительные аргументы сообщения</param>
		/// <returns></returns>
		Task ExecuteAsync(UserMessage userMessage, string args);
	}

}
