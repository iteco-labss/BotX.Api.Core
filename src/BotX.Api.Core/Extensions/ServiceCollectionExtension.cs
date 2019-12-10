using BotX.Api.Attributes;
using BotX.Api.StateMachine;
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
			externalServices.AddSingleton(typeof(IBotMessageSender), x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), ctsServiceUrl));
			externalServices.AddSingleton(typeof(IBotMessageSender), x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), ctsServiceUrl));
			externalServices.AddSingleton<ActionExecutor>();
			ConfigureBotActions(Assembly.GetEntryAssembly(), externalServices);

			return new ExpressBotService(botId, inChatExceptions, externalServices);
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

        public static void AddStateMachine<T>(this IServiceCollection externalServices) where T : BaseStateMachine
        {
            if (ExpressBotService.Configuration == null)
                throw new Exception($"Перед использованием {nameof(AddStateMachine)} необходимо сначала вызвать {nameof(AddExpressBot)}");

            if (!ExpressBotService.Configuration.StateMachines.Any(x => x == typeof(T)))
            {
                externalServices.AddTransient<T>();
                ExpressBotService.Configuration.StateMachines.Add(typeof(T));
            }

			var allStateTypes = Assembly.GetEntryAssembly().ExportedTypes
				.Where(x => x.IsSubclassOf(typeof(BaseState)));

			foreach (var t in allStateTypes)
				externalServices.AddTransient(t);
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
