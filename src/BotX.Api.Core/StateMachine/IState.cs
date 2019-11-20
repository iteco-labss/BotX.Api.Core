using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.StateMachine
{
	internal interface IState
	{
		BaseStateMachine StateMachine { get; }
		void SetContext(BaseStateMachine stage);
		Task StartAsync(UserMessage userMessage, dynamic model);
		Task ExecuteAsync(UserMessage userMessage, dynamic model);
	}
}
