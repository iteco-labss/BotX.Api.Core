using BotX.Api.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

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
		/// <param name="ctsServiceUrl">Адрес сервиса cts. Например https://cts.example.com</param>
		/// <param name="botId">Идентификатор бота</param>
		/// <param name="inChatExceptions">Нужно ли выводить сообщения об ошибках в чат</param>
		/// <returns></returns>
		public static ExpressBotService AddExpressBot(this IServiceCollection externalServices, 
			string ctsServiceUrl, Guid botId, bool inChatExceptions = false)
		{
			externalServices.AddRouting();
			externalServices.AddSingleton(x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), ctsServiceUrl));
			externalServices.AddSingleton(x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), ctsServiceUrl));
			externalServices.AddSingleton<ActionExecutor>();
			ConfigureBotActions(Assembly.GetEntryAssembly(), externalServices);

			return new ExpressBotService(botId, inChatExceptions, externalServices.BuildServiceProvider());
		}

		/// <summary>
		/// Добавляет поддержку BotX Api, позволяя создавать ботов для мессенджера Express. Без поддержки исходящих сообщений
		/// </summary>
		/// <param name="externalServices"></param>
		/// <param name="ctsServiceUrl">Адрес сервиса cts. Например https://cts.example.com</param>
		/// <param name="inChatExceptions">Нужно ли выводить сообщения об ошибках в чат</param>
		public static ExpressBotService AddExpressBot(this IServiceCollection externalServices,
			string ctsServiceUrl, bool inChatExceptions = false)
		{
			return AddExpressBot(externalServices, ctsServiceUrl, Guid.Empty, inChatExceptions);
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

			typesWithAttribute = applicationAssembly.GetExportedTypes()
				.Where(x => x.GetCustomAttribute(typeof(BotEventReceiverAttribute)) != null);

			foreach (var type in typesWithAttribute)
			{
				if (!services.Any(x => x.ImplementationType == type))
					services.AddTransient(type);

				ActionExecutor.AddEventReceiver(type);
			}

		}
	}
}
