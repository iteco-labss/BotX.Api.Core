using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotX.Api;
using BotX.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.ChatProcessing
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			if (!Uri.TryCreate(configuration["BOT_CTS"], UriKind.Absolute, out var cts))
				throw new Exception("The cts url could not be found. Please set the BOT_CTS variable in your 'User Secret' or Environment variables");

			if (!Guid.TryParse(configuration["BOT_ID"], out var botId))
				throw new Exception("The bot id could not be found. Please set the BOT_ID variable in your 'User Secret' or Environment variables");

			var secret = configuration["BOT_SECRET"];

			if (string.IsNullOrWhiteSpace(secret))
				throw new Exception("The bot secret could not be found. Please set the BOT_SECRET variable in your 'User Secret' or Environment variables");

			services.AddExpressBot(new BotXConfig(cts, botId, secret, true));
			services.AddMiddleware<HelloBotMiddleware>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseExpress();
		}
	}
}
