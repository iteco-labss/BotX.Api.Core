using BotX.Api.Middleware;
using BotX.Api.StateMachine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Extensions
{
	public static class ApplicationBuilderExtension
	{
       /// <summary>
       /// Подключает роутинг для обработки вызовов со стороны Express
       /// </summary>
       /// <param name="app"></param>
		public static void UseExpress(this IApplicationBuilder app)
		{
			app.UseMiddleware<RequestLoggingMiddleware>();
			var builder = new RouteBuilder(app);
			builder.MapMiddlewareGet("status", appBuilder => appBuilder.UseMiddleware<StatusMiddleware>());
			builder.MapMiddlewarePost("command", appBuilder => appBuilder.UseMiddleware<CommandMiddleware>());
			app.UseRouter(builder.Build());
		}
	}
}
