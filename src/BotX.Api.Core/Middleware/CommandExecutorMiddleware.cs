using BotX.Api.Delegates;
using BotX.Api.Executors;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BotX.Api.Middleware
{
	class CommandExecutorMiddleware
	{
		private readonly BotMiddlewareHandler next;
		public CommandExecutorMiddleware(BotMiddlewareHandler next, ILogger<CommandExecutorMiddleware> logger)
		{
			this.next = next;
		}

		public async Task InvokeAsync(UserMessage message, IServiceProvider serviceProvider)
		{
			var sender = ExpressBotService.Configuration.ServiceProvider.GetService<IBotMessageSender>();
			try
			{
				using var scope = serviceProvider.CreateScope();
				var actionExecutor = scope.ServiceProvider.GetService<ActionExecutor>();
				bool stateMachineLaunched = false;
				foreach (var smType in ExpressBotService.Configuration.StateMachines)
				{
					var machine = ExpressBotService.Configuration.ServiceProvider.GetService(smType) as BaseStateMachine;

					if (machine != null)
					{
						machine.UserMessage = message;
						machine.MessageSender = sender;

						var restored = machine.RestoreState();

						if (restored != null)
						{
							var state = ExpressBotService.Configuration.ServiceProvider.GetService(restored.State.GetType()) as BaseState;
							state.StateMachine = machine;
							if (state is BaseQuestionState && restored.State is BaseQuestionState)
								(state as BaseQuestionState).isOpen = (restored.State as BaseQuestionState).isOpen;

							machine.model = restored.model;
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

				if (!stateMachineLaunched)
					await actionExecutor.ExecuteAsync(message);
			}
			catch (Exception ex)
			{
				var config = ExpressBotService.Configuration.ServiceProvider.GetService<BotXConfig>();
				if (config.InChatExceptions == true)
					await sender.ReplyTextMessageAsync(message, ex.ToString());
				else
					throw;
			}
		}
	}
}
