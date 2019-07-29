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
	public class ActionExecutor
	{
		private static Dictionary<string, Type> actions = new Dictionary<string, Type>();
		internal static Dictionary<string, MethodInfo> actionEvents = new Dictionary<string, MethodInfo>();

		private readonly IServiceProvider serviceProvider;
		private readonly ILogger<ActionExecutor> logger;
		private const string UNNAMED_ACTION = "unnamed-action";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public ActionExecutor(IServiceProvider serviceProvider, ILogger<ActionExecutor> logger)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
		{
			this.serviceProvider = serviceProvider;
			this.logger = logger;
		}

		internal static void AddAction(string name, Type botActionClass) // where T : class, IBotAction
		{
			var attribute = botActionClass.GetCustomAttribute<BotActionAttribute>();
			if (attribute == null)
				throw new InvalidOperationException($"The type {botActionClass.Name} doesn't have the {nameof(BotActionAttribute)} attribute");

			var actionName = string.IsNullOrEmpty(attribute.Action) ? UNNAMED_ACTION : attribute.Action;
			actions.Add(name, botActionClass);
			ProcessEvents(actionName, botActionClass);
		}

		private static void ProcessEvents(string actionName, Type botActionClass)
		{
			var methods = botActionClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.GetCustomAttribute<BotActionEventAttribute>() != null)
				.ToDictionary(x => $"{actionName} {x.GetCustomAttribute<BotActionEventAttribute>().EventName}");

			foreach (var method in methods)
				actionEvents.Add(method.Key, method.Value);
		}

		internal async Task ExecuteAsync(UserMessage request)
		{
			logger.LogInformation("Enter the 'Execute' method");
			if (!request.Command.Body.StartsWith("/"))
				return;

			var msg = request.Command.Body.ToLower().Substring(1);
			var words = msg.Split(' ');
			var actionName = words.Length == 0 ? msg : words[0];

			if (actions.ContainsKey(actionName))
			{
				var action = (IBotAction)serviceProvider.GetService(actions[actionName]);

				string command = null;
				string[] args = null;

				if (words.Length > 1)
				{
					command = words[1];
					args = words.Skip(2).ToArray();
				}

				if (string.IsNullOrEmpty(command) || !actionEvents.ContainsKey($"{actionName} {command}"))
				{
					logger.LogInformation($"Executing {actionName} action");
					await action.ExecuteAsync(request, string.Join(' ', words.Skip(1)));
				}
				else
				{
					var @event = actionEvents[$"{actionName} {command}"];
					var eventInstance = Delegate.CreateDelegate(typeof(BotEventHandler), action, @event) as BotEventHandler;
					logger.LogInformation($"Executing {@event.Name} event inside the action {actionName}");
					await eventInstance(request, args);
				}
			}
		}
	}
}
