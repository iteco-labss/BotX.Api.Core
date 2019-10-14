using BotX.Api.JsonModel.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Middleware
{
	internal class StatusMiddleware
	{
		private readonly RequestDelegate next;

		public StatusMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger<StatusMiddleware> logger)
		{
			var response = new ResponseStatus
			{
				Status = "ok",
				Result = new ResponseResult
				{
					Enabled = true,
					Status_message = "hi",
					Commands = ExpressBotService.Configuration.Commands.ToArray()
				}
			};

			context.Response.ContentType = "text/json; charset=UTF-8";
			await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
			logger.LogInformation("return botx status");
			//await next.Invoke(context);
		}
	}
}
