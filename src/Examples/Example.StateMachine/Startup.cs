using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotX.Api;
using BotX.Api.Extensions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.StateMachine
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
			if (!Guid.TryParse(Environment.GetEnvironmentVariable("BOT_ID"), out var botId))
				throw new Exception("The bot id could not be found. Please set BOT_ID environment variable");

			var secret = Environment.GetEnvironmentVariable("BOT_SECRET");
			if (string.IsNullOrWhiteSpace(secret))
				throw new Exception("The secret key could not be found. Please set BOT_SECRET environment variable");

			if (botId == Guid.Empty && secret == "your_secret_key")
				throw new Exception("Please set your bot id in launchSettings.json");

			services.AddExpressBot(new BotXConfig(botId, secret, true));
			services.AddStateMachine<DemoStateMachine>();
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
