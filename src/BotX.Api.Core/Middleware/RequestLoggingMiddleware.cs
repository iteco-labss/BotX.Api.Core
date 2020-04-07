using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace BotX.Api.Middleware
{
	public class RequestLoggingMiddleware
	{
		private readonly RequestDelegate next;
		private const int maxLength = 1024 * 64;

		public RequestLoggingMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
		{
			try
			{
				context.Request.EnableBuffering();
				logger.LogInformation($"Request {context.Request.Method} {context.Request.GetDisplayUrl()}");

				if (context.Request.ContentLength != null)
				{
					StreamReader reader = new StreamReader(context.Request.Body);
					string body = string.Empty;
					if (context.Request.ContentLength < maxLength)
						body = await reader.ReadToEndAsync();
					else
					{
						var buffer = new char[maxLength];
						await reader.ReadAsync(buffer, 0, maxLength);
						body = new string(buffer) + "... (the message was trimmed)";
					}

					logger.LogInformation(body);
				}
			}
			finally
			{
				if (next != null)
					await next.Invoke(context);
			}
		}
	}
}
