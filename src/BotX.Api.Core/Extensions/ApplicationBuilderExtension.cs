using BotX.Api.Executors;
using BotX.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BotX.Api.Extensions
{
	/// <summary>
	/// Расширение для IApplicationBuilder, добавляющее поддержку Express
	/// </summary>
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

			var middlewareService = app.ApplicationServices.GetService<MiddlewareExecutor>();
			middlewareService.CreateChainMiddlewareCall();
		}
	}
}
