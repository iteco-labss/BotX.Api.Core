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
			var cts = Environment.GetEnvironmentVariable("ctsserviceaddress", EnvironmentVariableTarget.Machine);
			if (string.IsNullOrEmpty(cts))
				throw new Exception("cts server address is not found");

			// example bot configuration with authorization 
			services.AddExpressBot(new BotXConfig()
			{
				CtsServiceUrl = cts,
				BotId = new Guid("botId"),
				SecretKey = "SecretKey",
				InChatExceptions = true
			});

			//services.AddExpressBot(new BotXConfig()
			//{
			//	CtsServiceUrl = cts,
			//	InChatExceptions = true
			//}).AddBaseCommand("sayhello", "скажи привет")
			//	.AddBaseCommand("saydate", "скажи дату");
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseExpress();
		}
	}
}
