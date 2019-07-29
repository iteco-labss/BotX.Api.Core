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
		/// <param name="title">Название команды</param>
		/// <param name="description">Описание команды</param>
		public ExpressBotService AddBaseCommand(string title, string description)
		{
			commands.Add(new Command { Name = title, Body = title, Description = description, Options = new Option { Clickable = true } });
			return this;
		}
	}
}
