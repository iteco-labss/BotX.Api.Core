using BotX.Api.JsonModel.Menu;
using BotX.Api.JsonModel.Response;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api
{
	/// <summary>
	/// Класс, позволяющий задать конфигурацию бота
	/// </summary>
	public class ExpressBotService
	{
		/// <summary>
		/// Конфигурация бота
		/// </summary>
		public static ExpressBotService Configuration { get; private set; }

		internal List<Command> Commands { get; private set; }
        internal List<Type> StateMachines { get; private set; }
		internal bool ThrowExceptions { get; private set; }
		internal IServiceProvider ServiceProvider { get; }
		internal Guid BotId { get; private set; }

		internal ExpressBotService(Guid botId, bool throwExceptions, IServiceProvider serviceProvider)
		{
			BotId = botId;
			ThrowExceptions = throwExceptions;
			ServiceProvider = serviceProvider;
			Commands = new List<Command>();
            StateMachines = new List<Type>();
			Configuration = this;
		}

		/// <summary>
		/// Добавляет команду для бота
		/// </summary>
		/// <param name="command">Имя команды</param>
		/// <param name="description">Описание команды</param>
		public ExpressBotService AddBaseCommand(string command, string description)
		{
			Commands.Add(new Command { Name = command, Body = command, Description = description, Options = new Option { Clickable = true } });
			return this;
		}

        public void AddStateMachine<T>() where T : BaseStateMachine
        {
        }
    }
}
