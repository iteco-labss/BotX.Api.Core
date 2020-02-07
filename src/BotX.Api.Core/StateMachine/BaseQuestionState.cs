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

		internal override async Task StartAsync(UserMessage userMessage)
		{
			if (!isOpen)
				await WelcomeAsync();
			else
				await ExecuteAsync();

			isOpen = !isOpen;
		}

		/// <summary>
		/// Реализует логику события перехода в состояние. Тут можно спросить пользователя о чём-то
		/// </summary>
		/// <returns></returns>
		public abstract Task WelcomeAsync();

		/// <summary>
		/// Вызывается при сбросе состояния Стейт машины
		/// </summary>
		public override void ResetState()
		{
			isOpen = false;
			base.ResetState();
		}
	}
}
