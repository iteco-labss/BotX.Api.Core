using BotX.Api.JsonModel.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.StateMachine
{
    /// <summary>
    /// Реализует одно состояние для конечного автомата <seealso cref="BaseStateMachine"/>
    /// </summary>
    public abstract class BaseQuestionState : BaseState
    {
		[JsonProperty]
		internal bool isOpen = false;

		internal override async Task StartAsync(UserMessage userMessage, dynamic model)
		{
			if (!isOpen && !StateMachine.isFinished)
				await WelcomeAsync(userMessage, model);
			else
				await ExecuteAsync(userMessage, model);

			isOpen = !isOpen;
		}

		/// <summary>
		/// Реализует логику события перехода в состояние. Тут можно спросить пользователя о чём-то
		/// </summary>
		/// <param name="userMessage">Входящее сообщение от пользователя</param>
		/// <param name="model">Модель данных, формируемая конечным автоматом (передаётся между всеми состояниями)</param>
		/// <returns></returns>
		public abstract Task WelcomeAsync(UserMessage userMessage, dynamic model);        
	}
}
