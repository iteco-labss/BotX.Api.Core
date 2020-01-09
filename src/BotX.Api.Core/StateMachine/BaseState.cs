using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotX.Api.JsonModel.Request;
using Newtonsoft.Json;

namespace BotX.Api.StateMachine
{
	public abstract class BaseState
	{
		/// <summary>
		/// Ссылка на конечный автомат, к которому относится данное состояние
		/// </summary>
		[JsonIgnore]
		public BaseStateMachine StateMachine { get; internal set; }
		
		internal void SetContext(BaseStateMachine machine)
		{
			StateMachine = machine;
		}

		internal virtual async Task StartAsync(UserMessage userMessage, dynamic model)
		{
			if (!StateMachine.isFinished)
				await ExecuteAsync(model);
		}

		/// <summary>
		/// Реализует логику данного состояния. Тут можно обработать ответ пользователя
		/// </summary>
		/// <param name="model">Модель данных, формируемая конечным автоматом (передаётся между всеми состояниями)</param>
		/// <returns></returns>
		public abstract Task ExecuteAsync(dynamic model);
	}
}
