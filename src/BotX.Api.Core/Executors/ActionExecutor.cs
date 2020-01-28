﻿using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.Delegates;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Executors
{
	/// <summary>
	/// Класс, производящий маршрутизацию команд бота между контроллерами
	/// </summary>
	internal sealed class ActionExecutor : IDisposable
	{
		private static Dictionary<string, Type> actions = new Dictionary<string, Type>();
		private static HashSet<Type> unnamedActions = new HashSet<Type>();
		internal static Dictionary<string, EventData> actionEvents = new Dictionary<string, EventData>();

		private readonly IServiceScope scope;
		private readonly ILogger<ActionExecutor> logger;

		public ActionExecutor(IServiceScopeFactory serviceScopeFactory, ILogger<ActionExecutor> logger)
		{
			scope = serviceScopeFactory.CreateScope();
			this.logger = logger;
		}

		internal static void AddAction(string name, Type botActionClass)
		{
			var attribute = botActionClass.GetCustomAttribute<BotActionAttribute>();
			if (attribute == null)
				throw new InvalidOperationException($"The type {botActionClass.Name} doesn't have the {nameof(BotActionAttribute)} attribute");

			actions.Add(name, botActionClass);
		}

		internal static void AddUnnamedAction(Type botActionClass)
		{
			unnamedActions.Add(botActionClass);
			if (!actions.ContainsKey(botActionClass.Name.ToLower()))
				actions.Add(botActionClass.Name.ToLower(), botActionClass);
		}

		internal static void AddEventReceiver(Type botEventReceiverClass)
		{
			if (!actions.ContainsKey(botEventReceiverClass.Name.ToLower()))
			{
				actions.Add(botEventReceiverClass.Name.ToLower(), botEventReceiverClass);
			}
		}

		internal static void RegisterEvents(Assembly applicationAssembly, IServiceCollection services)
		{
			foreach (var eventClass in applicationAssembly.GetExportedTypes())
			{
				var methods = eventClass.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(x => x.GetCustomAttribute<BotButtonEventAttribute>() != null);
				if (methods.Count() == 0)
					continue;

				foreach (var method in methods)
				{
					string eventName = method.GetCustomAttribute<BotButtonEventAttribute>()?.EventName;
					if (string.IsNullOrEmpty(eventName))
						throw new InvalidOperationException($"'EventName' is required with 'BotButtonEventAttribute'");
					if (actionEvents.ContainsKey(eventName))
						throw new InvalidOperationException($"Event with name '{eventName}' already exists");

					var types = method.GetParameters().Select(p => p.ParameterType).Concat(new[] { method.ReturnType }).ToArray();
					actionEvents.Add(eventName, new EventData()
					{
						Event = method,
						EventClass = eventClass,
						EventInstanse = new FastMethodInfo(method),
						DelegateType = Expression.GetFuncType(types)
					});
				}

				if (!services.Any(x => x.ImplementationType == eventClass))
					services.AddTransient(eventClass);
			}
		}

		/// <summary>
		/// Вызывает нужное событие, если пользователь его сгенерировал
		/// </summary>
		/// <param name="request"></param>
		/// <returns>true - если событие было обработаано, false - если нет</returns>
		internal async Task<bool> ExecuteEventAsync(UserMessage request, object instance = null)
		{
			if (string.IsNullOrEmpty(request.Command.Data?.EventType))
				return false;

			var payload = JsonConvert.DeserializeObject<Payload>(request.Command.Data?.Payload, new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.All,
				MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
			});

			await InvokeEvent(request, actionEvents[request.Command.Data.EventType], payload, instance);
			return true;
		}

		internal async Task ExecuteAsync(UserMessage request)
		{
			logger.LogInformation("Enter the 'Execute' method");

			if (request.Command.Body.Length == 0)
			{
				logger.LogInformation("The message is empty");
				return;
			}
			if (await ExecuteEventAsync(request))
				return;

			bool messageIsAction = request.Command.Body.StartsWith('/');
			var msg = request.Command.Body.ToLower().Substring(1);
			var words = messageIsAction ? msg.Split(' ') : new string[0];
			var actionName = words.Length == 0 ? string.Empty : words.First();

			string[] args = null;

			if (words.Length > 1)
			{
				args = words.Skip(1).ToArray();
			}

			if (!string.IsNullOrEmpty(actionName) && actions.ContainsKey(actionName))
				await InvokeNamedAction(request, actionName, args);
			else
				await InvokeUnnamedAction(request);
		}

		private async Task InvokeNamedAction(UserMessage request, string actionName, string[] args)
		{
			logger.LogInformation("Enter InvokeNamedAction");
			try
			{
				var action = (IBotAction)scope.ServiceProvider.GetService(actions[actionName]);
				await action.InternalExecuteAsync(request, args);
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex.ToString());
				throw;
			}
		}

		private async Task InvokeEvent(UserMessage request, EventData @event, Payload payload, object instance)
		{
			logger.LogInformation("Enter InvokeEvent");
			if (instance == null)
				instance = scope.ServiceProvider.GetService(@event.EventClass);

			object[] param = new object[] { request, payload };
			await @event.EventInstanse.InvokeAsync(instance, param);
		}

		private async Task InvokeUnnamedAction(UserMessage request)
		{
			logger.LogInformation("Enter InvokeUnnamedAction");
			foreach (var actionType in unnamedActions)
			{
				var action = (IBotAction)scope.ServiceProvider.GetService(actionType);
				await action.InternalExecuteAsync(request, null);
			}
		}

		public void Dispose()
		{
			scope.Dispose();
		}

	}

	internal class EventData
	{
		internal Type EventClass { get; set; }
		internal MethodInfo Event { get; set; }
		internal FastMethodInfo EventInstanse { get; set; }
		internal Type DelegateType { get; set; }
	}

	
}
