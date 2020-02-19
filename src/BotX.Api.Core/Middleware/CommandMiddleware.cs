using BotX.Api.Executors;
using BotX.Api.JsonModel.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
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

		public async Task InvokeAsync(HttpContext context, ILogger<CommandMiddleware> logger, IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var processingMiddleware = scope.ServiceProvider.GetRequiredService<MiddlewareExecutor>();
				logger.LogInformation("message has been received");
				context.Request.Body.Position = 0;
				StreamReader reader = new StreamReader(context.Request.Body);
				var body = await reader.ReadToEndAsync();
				var message = JsonConvert.DeserializeObject<UserMessage>(body);
				if (message == null)
					throw new FormatException("body is not UserMessage");
				_ = processingMiddleware.RunMiddlewareAsync(message).ConfigureAwait(false);
			}

			context.Response.StatusCode = StatusCodes.Status200OK;
		}
	}
}
