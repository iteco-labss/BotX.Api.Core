using BotX.Api.JsonModel.Menu;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api
{
	/// <summary>
	/// Конфигурация бота
	/// </summary>
	public class ExpressBotService
	{
		internal static readonly List<Command> commands = new List<Command>();

		/// <summary>
		/// Добавляет команду для бота
		/// </summary>
		/// <param name="command">Имя команды</param>
		/// <param name="description">Описание команды</param>
		public ExpressBotService AddBaseCommand(string command, string description)
		{
			commands.Add(new Command { Name = command, Body = command, Description = description, Options = new Option { Clickable = true } });
			return this;
		}
	}
}
