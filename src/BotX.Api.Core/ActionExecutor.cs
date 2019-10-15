using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.Delegates;
using BotX.Api.Internal;
using BotX.Api.JsonModel.Request;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
	public class ActionExecutor : IDisposable
	{
		private static Dictionary<string, Type> actions = new Dictionary<string, Type>();
		private static HashSet<Type> unnamedActions = new HashSet<Type>();
		internal static Dictionary<string, MethodInfo> actionEvents = new Dictionary<string, MethodInfo>();

		private readonly IServiceProvider serviceProvider;
		private readonly ILogger<ActionExecutor> logger;
		private readonly IServiceScope scope;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public ActionExecutor(IServiceProvider serviceProvider, ILogger<ActionExecutor> logger)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
		{
			this.serviceProvider = serviceProvider;
			this.logger = logger;
			this.scope = serviceProvider.CreateScope();
		}

		internal static void AddAction(string name, Type botActionClass) // where T : class, IBotAction
		{
			var attribute = botActionClass.GetCustomAttribute<BotActionAttribute>();
			if (attribute == null)
				throw new InvalidOperationException($"The type {botActionClass.Name} doesn't have the {nameof(BotActionAttribute)} attribute");

			actions.Add(name, botActionClass);
			ProcessEvents(name, botActionClass);
		}

		internal static void AddUnnamedAction(Type botActionClass)
		{
			unnamedActions.Add(botActionClass);
			actions.Add(botActionClass.Name.ToLower(), botActionClass);
			ProcessEvents(botActionClass.Name.ToLower(), botActionClass);
		}

		internal static void AddEventReceiver(Type botEventReceiverClass)
		{
			actions.Add(botEventReceiverClass.Name.ToLower(), botEventReceiverClass);
			ProcessEvents(botEventReceiverClass.Name.ToLower(), botEventReceiverClass);
		}

		internal static string MakeEventKey(string actionName, string eventName)
		{
			return $"{actionName} {eventName}";
		}

		private static void ProcessEvents(string actionName, Type botActionClass)
		{
			var methods = botActionClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.GetCustomAttribute<BotButtonEventAttribute>() != null)
				.ToDictionary(x => MakeEventKey(actionName, x.GetCustomAttribute<BotButtonEventAttribute>().EventName));

			foreach (var method in methods)
				actionEvents.Add(method.Key, method.Value);
		}

		internal async Task ExecuteSystemAsync(UserMessage request)
		{
			throw new NotImplementedException();
		}

		internal async Task ExecuteAsync(UserMessage request)
		{
			logger.LogInformation("Enter the 'Execute' method");

			if (request.Command.Body.Length == 0)
			{
				logger.LogInformation("Message is empty");
				return;
			}

			bool messageIsAction = request.Command.Body.StartsWith('/');
			var msg = request.Command.Body.ToLower().Substring(1);
			var words = messageIsAction ? msg.Split(' ') : new string[0];
			var actionName = words.Length == 0 ? string.Empty : words.First();

			string command = null;
			string[] args = null;

			if (words.Length > 1)
			{
				command = words[1];
				args = words.Skip(1).ToArray();
			}

			var commandKey = MakeEventKey(actionName, command);

			if (!string.IsNullOrEmpty(actionName) && actions.ContainsKey(actionName))
			{
				if (string.IsNullOrEmpty(command) || !actionEvents.ContainsKey(commandKey))
					await InvokeNamedAction(request, actionName, args);
				else
					await InvokeEvent(
						request: request, 
						actionName: actionName, 
						@event: actionEvents[commandKey], 
						args: args.Skip(1).ToArray());
			}
			else if (!actionEvents.ContainsKey(commandKey))
				await InvokeUnnamedAction(request);
		}

		private async Task InvokeNamedAction(UserMessage request, string actionName, string[] args)
		{
			logger.LogInformation("Enter InvokeNamedAction");
			try
			{				
				var action = (IBotAction)scope.ServiceProvider.GetService(actions[actionName]);
				await action.ExecuteAsync(request, args);
			}
			catch(Exception ex)
			{
				logger.LogCritical(ex.ToString());
				throw;
			}
		}

		private async Task InvokeEvent(UserMessage request, string actionName, MethodInfo @event, string[] args)
		{
			logger.LogInformation("Enter InvokeEvent");
			var action = scope.ServiceProvider.GetService(actions[actionName]);
			var eventInstance = Delegate.CreateDelegate(typeof(BotEventHandler), action, @event) as BotEventHandler;
			await eventInstance(request, args);
		}

		private async Task InvokeUnnamedAction(UserMessage request)
		{
			logger.LogInformation("Enter InvokeUnnamedAction");
			foreach (var actionType in unnamedActions)
			{
				var action = (IBotAction)scope.ServiceProvider.GetService(actionType);
				await action.ExecuteAsync(request, null);
			}
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public void Dispose()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
		{
			this.scope.Dispose();
		}
	}
}
