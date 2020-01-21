using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Delegates
{
	/// <summary>
	/// Метод события контроллера BotAction
	/// </summary>
	/// <param name="userMessage">Входящее сообщение от пользователя</param>
	/// <param name="args">Дополнительные аргументы, которые были заданы кнопке при её создании</param>
	/// <returns></returns>
	public delegate Task BotEventHandler(UserMessage userMessage, Payload payload);
}
