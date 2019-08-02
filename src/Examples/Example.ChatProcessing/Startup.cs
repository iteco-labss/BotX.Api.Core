using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			var mvc = services.AddMvc();
			var cts = Environment.GetEnvironmentVariable("ctsserviceaddress", EnvironmentVariableTarget.Machine);
			services.AddExpressBot(mvc, cts, Guid.Parse("49de73fe-e3a4-5231-9972-f6690ad28a25"), true)
				.AddBaseCommand("sayhello", "скажи привет")
				.AddBaseCommand("saydate", "скажи дату");
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
