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
				BotId = new Guid("807c20c0-a1d0-54df-b46b-41ddf4f02dd7"), //Change this with your botid
				SecretKey = "6311c6234d1b371b62f964527fed3c93", // Change this with your secret key
				InChatExceptions = true
			});
			//services.AddExpressBot(new BotXConfig()
			//{
			//	CtsServiceUrl = cts,
			//	InChatExceptions = true
			//}).AddBaseCommand("sayhello", "скажи привет")
			//	.AddBaseCommand("saydate", "скажи дату");

			services.AddMiddleware<HelloBotMiddleware>();
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
