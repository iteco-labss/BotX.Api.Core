using BotX.Api.Abstract;
using BotX.Api.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BotX.Api.Extensions
{
	/// <summary>
	/// Класс расширения IServiceCollection
	/// </summary>
	public static class ServiceCollectionExtension
	{
		/// <summary>
		/// Добавляет поддержку BotX Api, позволяя создавать ботов для мессенджера Express
		/// </summary>
		/// <param name="externalServices"></param>
		/// <param name="mvcBuilder"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static ExpressBotService AddExpressBot(this IServiceCollection externalServices, IMvcBuilder mvcBuilder, string server)
		{
			mvcBuilder.AddApplicationPart(Assembly.Load(new AssemblyName("BotX.Api")));
			externalServices.AddSingleton(x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), server));
			InternalContainer.ServiceCollection.AddSingleton(x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), server));
			InternalContainer.ServiceCollection.AddSingleton<ActionExecutor>();
			ConfigureBotActions(Assembly.GetEntryAssembly(), externalServices);

			return new ExpressBotService();
		}

		private static void ConfigureBotActions(Assembly applicationAssembly, IServiceCollection services)
		{
			services.AddSingleton<ActionExecutor>();
			var typesWithAttribute = applicationAssembly.GetExportedTypes()
				.Where(x => x.GetCustomAttribute(typeof(BotActionAttribute)) != null);

			foreach (var type in typesWithAttribute)
			{
				var att = type.GetCustomAttribute<BotActionAttribute>();
				services.AddTransient(type);

				if (att.IsCommon)
					ActionExecutor.AddUnnamedAction(type);
				else
					ActionExecutor.AddAction(att.Action, type);
			}
		}
	}
}
