using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Middleware
{
	public class RequestLoggingMiddleware
	{
		private readonly RequestDelegate next;

		public RequestLoggingMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
		{
			try
			{
				context.Request.EnableBuffering();
				StreamReader reader = new StreamReader(context.Request.Body);
				string body = await reader.ReadToEndAsync();
				logger.LogInformation(body);
			}
			finally
			{
				if (next != null)
					await next.Invoke(context);
			}
		}
	}
}
