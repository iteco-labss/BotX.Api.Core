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

		/// <summary>
		/// Текущий стейт
		/// </summary>
		[JsonIgnore]
		public dynamic Model { get => StateMachine.Model; set => StateMachine.Model = value; }
		
		internal void SetContext(BaseStateMachine machine)
		{
			StateMachine = machine;
		}

		internal virtual async Task StartAsync(UserMessage userMessage)
		{
			if (!StateMachine.isFinished)
				await ExecuteAsync();
		}

		/// <summary>
		/// Реализует логику данного состояния. Тут можно обработать ответ пользователя
		/// </summary>
		/// <returns></returns>
		public abstract Task ExecuteAsync();

		/// <summary>
		/// Вызывается при сбросе состояния Стейт машины
		/// </summary>
		public abstract void ResetState();
	}
}
