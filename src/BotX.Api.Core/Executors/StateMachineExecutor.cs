using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Executors
{
	internal class StateMachineExecutor : IDisposable
	{
		internal static List<Type> StateMachines { get; private set; } = new List<Type>();

		private readonly IServiceScope scope;
		private readonly ILogger<StateMachineExecutor> logger;
		public StateMachineExecutor(IServiceScopeFactory serviceScopeFactory, ILogger<StateMachineExecutor> logger)
		{
			scope = serviceScopeFactory.CreateScope();
			this.logger = logger;
		}

		internal async Task<bool> ExecuteAsync(UserMessage message)
		{
			var sender = scope.ServiceProvider.GetService<IBotMessageSender>();
			bool stateMachineLaunched = false;
			foreach (var smType in StateMachines)
			{
				var machine = scope.ServiceProvider.GetService(smType) as BaseStateMachine;

				if (machine != null)
				{
					machine.UserMessage = message;
					machine.MessageSender = sender;

					var restored = machine.RestoreState();

					if (restored != null)
					{
						var state = scope.ServiceProvider.GetService(restored.State.GetType()) as BaseState;
						state.StateMachine = machine;
						if (state is BaseQuestionState && restored.State is BaseQuestionState)
							(state as BaseQuestionState).isOpen = (restored.State as BaseQuestionState).isOpen;

						machine.Model = restored.Model;
						machine.firstStep = restored.firstStep;
						machine.isFinished = restored.isFinished;
						machine.State = state;
						stateMachineLaunched = !machine.isFinished;
						if (stateMachineLaunched)
							await machine.EnterAsync(message);
						break;
					}
				}
			}

			return stateMachineLaunched;
		}

		public void Dispose()
		{
			scope.Dispose();
		}
	}
}
