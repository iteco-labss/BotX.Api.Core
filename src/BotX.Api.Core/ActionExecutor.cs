using BotX.Api.Abstract;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api
{
	/// <summary>
	/// Класс, производящий маршрутизацию команд бота между контроллерами
	/// </summary>
	internal sealed class ActionExecutor
	{
		private static Dictionary<string, Type> actions = new Dictionary<string, Type>();
		private static HashSet<Type> unnamedActions = new HashSet<Type>();
		internal static Dictionary<string, EventValue> actionEvents = new Dictionary<string, EventValue>();

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
			AddEvent(botActionClass);
		}

		internal static void AddUnnamedAction(Type botActionClass)
		{
			unnamedActions.Add(botActionClass);
			if (!actions.ContainsKey(botActionClass.Name.ToLower()))
				actions.Add(botActionClass.Name.ToLower(), botActionClass);
			AddEvent(botActionClass);
		}

		internal static void AddEventReceiver(Type botEventReceiverClass)
		{
			if (!actions.ContainsKey(botEventReceiverClass.Name.ToLower()))
			{
				actions.Add(botEventReceiverClass.Name.ToLower(), botEventReceiverClass);
				AddEvent(botEventReceiverClass);
			}
		}

		internal static void AddEvent(Type eventClass)
		{
			var methods = eventClass.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.GetCustomAttribute<BotButtonEventAttribute>() != null);

			foreach (var method in methods)
			{
				string eventName = method.GetCustomAttribute<BotButtonEventAttribute>()?.EventName;
				if (string.IsNullOrEmpty(eventName))
					throw new InvalidOperationException($"'EventName' is required with 'BotButtonEventAttribute'");
				if (actionEvents.ContainsKey(eventName))
					throw new InvalidOperationException($"Event with name '{eventName}' already exists");

				actionEvents.Add(eventName, new EventValue()
				{
					Event = method,
					EventClass = eventClass,
				});
			}
		}

		internal async Task ExecuteAsync(UserMessage request)
		{
			logger.LogInformation("Enter the 'Execute' method");

			if (request.Command.Body.Length == 0)
			{
				logger.LogInformation("The message is empty");
				return;
			}
			if (!string.IsNullOrEmpty(request.Command.Data?.EventType))
			{
				var payload = JsonConvert.DeserializeObject<Payload>(request.Command.Data?.Payload, new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All,
					MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
				});

				await InvokeEvent(request, actionEvents[request.Command.Data.EventType], payload);
				return;
			}

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

		private async Task InvokeEvent(UserMessage request, EventValue @event, Payload payload)
		{
			logger.LogInformation("Enter InvokeEvent");
			var action = scope.ServiceProvider.GetService(@event.EventClass);
			var eventInstance = Delegate.CreateDelegate(typeof(BotEventHandler), action, @event.Event) as BotEventHandler;
			await eventInstance(request, payload);
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
	}

	internal class EventValue
	{
		internal Type EventClass { get; set; }
		internal MethodInfo Event { get; set; }
	}
}
