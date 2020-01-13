using BotX.Api.Configuration;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Middleware
{
	internal class CommandMiddleware
	{
		private readonly RequestDelegate next;

		public CommandMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger<CommandMiddleware> logger, IServiceProvider serviceProvider, IBotMessageSender sender, BotXConfig botXConfig)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var actionExecutor = scope.ServiceProvider.GetService<ActionExecutor>();
				logger.LogInformation("message has been received");
				context.Request.Body.Position = 0;
				StreamReader reader = new StreamReader(context.Request.Body);
				var body = await reader.ReadToEndAsync();
				var message = JsonConvert.DeserializeObject<UserMessage>(body);
				if (message == null)
					throw new FormatException("body is not UserMessage");

				_ = Task.Run(async () =>
				  {
					  try
					  {
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
						  if (botXConfig.InChatExceptions == true)
						  {
							  await sender.ReplyTextMessageAsync(message, ex.ToString());
							  throw;
						  }
					  }
				  });
			}

			context.Response.StatusCode = StatusCodes.Status200OK;
		}
	}
}
