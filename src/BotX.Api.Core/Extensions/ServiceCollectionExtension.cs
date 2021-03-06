﻿using BotX.Api.Attributes;
using BotX.Api.HttpClients;
using BotX.Api.StateMachine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using BotX.Api.Executors;

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
		/// <param name="config">Конфиг для BotX Api</param>
		/// <returns></returns>
		public static ExpressBotService AddExpressBot(this IServiceCollection externalServices, BotXConfig config)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (config.BotEntries.Count == 0)
				throw new ArgumentException("Configuration doesn't contain a bot entry. Array must contain at least 1 item");

			externalServices.AddRouting();
			externalServices.AddSingleton(config);

			externalServices.AddHttpClient<IBotXHttpClient, BotXHttpClient>();
			externalServices.AddSingleton(typeof(IBotMessageSender), x => new BotMessageSender(x.GetService<ILogger<BotMessageSender>>(), x.GetService<IBotXHttpClient>()));
			externalServices.AddSingleton<ActionExecutor>();
			externalServices.AddSingleton<MiddlewareExecutor>();
			externalServices.AddSingleton<StateMachineExecutor>();
			ConfigureBotActions(Assembly.GetEntryAssembly(), externalServices);

			return new ExpressBotService(config.InChatExceptions);
		}

		/// <summary>
		/// Добавляем обработчик, который вызывается до срабатывания эвента, экшена или стейт машины.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="externalServices"></param>
		public static void AddMiddleware<T>(this IServiceCollection externalServices)
		{
			MiddlewareExecutor.AddMiddleware(typeof(T));
		}

		public static void AddStateMachine<T>(this IServiceCollection services) where T : BaseStateMachine
		{
			if (ExpressBotService.Configuration == null)
				throw new Exception($"Перед использованием {nameof(AddStateMachine)} необходимо сначала вызвать {nameof(AddExpressBot)}");

			if (!StateMachineExecutor.StateMachines.Any(x => x == typeof(T)))
			{
				services.AddTransient<T>();
				StateMachineExecutor.StateMachines.Add(typeof(T));
			}

			var allStateTypes = Assembly.GetEntryAssembly().ExportedTypes
				.Where(x => x.IsSubclassOf(typeof(BaseState)));

			foreach (var type in allStateTypes)
				if (!services.Any(x => x.ImplementationType == type))
					services.AddTransient(type);
		}

		private static void ConfigureBotActions(Assembly applicationAssembly, IServiceCollection services)
		{
			services.AddScoped<ActionExecutor>();
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
			ActionExecutor.RegisterEvents(applicationAssembly, services);

		}
	}
}
