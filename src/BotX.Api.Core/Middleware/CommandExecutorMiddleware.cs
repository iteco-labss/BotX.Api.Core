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

		public async Task InvokeAsync(UserMessage message, IServiceScopeFactory serviceScopeFactory)
		{
			using var scope = serviceScopeFactory.CreateScope();
			try
			{
				var actionExecutor = scope.ServiceProvider.GetService<ActionExecutor>();
				var stateMachineExecutor = scope.ServiceProvider.GetService<StateMachineExecutor>();

				if (!string.IsNullOrEmpty(message.Command.Data?.EventType))
				{
					await actionExecutor.ExecuteEventAsync(message);
					return;
				}
				bool stateMachineLaunched = await stateMachineExecutor.ExecuteAsync(message);

				if (!stateMachineLaunched)
					await actionExecutor.ExecuteAsync(message);
			}
			catch (Exception ex)
			{
				var config = scope.ServiceProvider.GetService<BotXConfig>();
				var sender = scope.ServiceProvider.GetService<IBotMessageSender>();
				if (config.InChatExceptions == true)
					await sender.ReplyTextMessageAsync(message, ex.ToString());
				else
					throw;
			}
		}
	}
}
