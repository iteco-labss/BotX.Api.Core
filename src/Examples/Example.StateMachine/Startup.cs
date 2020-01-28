using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotX.Api;
using BotX.Api.Extensions;
using Example.ChatProcessing.Bot.StateMachine;
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
			var cts = Environment.GetEnvironmentVariable("ctsserviceaddress", EnvironmentVariableTarget.Machine);
			if (string.IsNullOrEmpty(cts))
				throw new Exception("cts server address is not found");

			services.AddExpressBot(new BotXConfig()
			{
				CtsServiceUrl = cts,
				BotId = new Guid("BOTID"),
				SecretKey = "SecretKey",
				InChatExceptions = true
			}).AddBaseCommand("start", "State machine");
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
