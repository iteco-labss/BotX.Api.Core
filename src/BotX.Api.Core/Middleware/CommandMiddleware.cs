using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Microsoft.AspNetCore.Http;
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

		public async Task InvokeAsync(HttpContext context, ILogger<CommandMiddleware> logger, ActionExecutor actionExecutor, BotMessageSender sender)
		{
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
						  machine.UserMessage = message;

						  if (machine != null)
						  {
							  var restored = machine.RestoreState();
							  if (restored != null)
							  {
								  stateMachineLaunched = true;
								  await restored.EnterAsync(message);
								  break;
							  }
						  }
					  }

					  if (!stateMachineLaunched)
						  await actionExecutor.ExecuteAsync(message);
				  }
				  catch (Exception ex)
				  {
					  await sender.ReplyTextMessageAsync(message, ex.ToString());
					  throw;
				  }
			  });

			context.Response.StatusCode = StatusCodes.Status200OK;
		}
	}
}
