using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Delegates
{
	/// <summary>
	/// Делегат для вызова следующего обработчика в цепочке Middleware
	/// </summary>
	/// <param name="message">Входящее сообщение</param>
	/// <returns></returns>
	public delegate Task BotMiddlewareHandler(UserMessage message);
}
