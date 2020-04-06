using BotX.Api.Delegates;
using BotX.Api.JsonModel.Request;
using BotX.Api.Middleware;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BotX.Api.Executors
{
	internal sealed class MiddlewareExecutor
	{
		static List<MiddlewareData> Middlewares { get; set; } = new List<MiddlewareData>();

		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly ILogger<MiddlewareExecutor> logger;

		public MiddlewareExecutor(IServiceScopeFactory serviceScopeFactory, ILogger<MiddlewareExecutor> logger)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.logger = logger;
		}

		/// <summary>
		/// Добавляем обработчик в список обработчиков.
		/// </summary>
		/// <param name="middlewareClass"></param>
		public static void AddMiddleware(Type middlewareClass)
		{
			var method = middlewareClass.GetMethod("Invoke") ?? middlewareClass.GetMethod("InvokeAsync");
			if (method == null)
			{
				throw new Exception($"{middlewareClass.Name} does not contain a method 'Invoke' or 'InvokeAsync'");
			}
			if (!method.GetParameters().Any(x => x.ParameterType == typeof(UserMessage)))
			{
				throw new Exception($"UserMessage is require param");
			}
			var middleware = new MiddlewareData();
			middleware.Class = middlewareClass;
			middleware.Method = new FastMethodInfo(method);
			middleware.Parameters = method.GetParameters().Select(x => x.ParameterType).ToList();
			Middlewares.Add(middleware);
		}

		/// <summary>
		/// Создает из списка обработчиков пайплайн обработки входящего запроса.
		/// Последним обработчиком всегда будет являться <see cref="CommandExecutorMiddleware"/>
		/// </summary>
		public void CreateChainMiddlewareCall()
		{
			// Добавляем CommandExecutorMiddleware, она отвечает за маршрутизацию экшенов, эвентов и стейт машины
			MiddlewareExecutor.AddMiddleware(typeof(CommandExecutorMiddleware));

			using var scope = serviceScopeFactory.CreateScope();
			MiddlewareData prevMiddleware = null;
			foreach (var middleware in Enumerable.Reverse(Middlewares))
			{
				var constructor = middleware.Class.GetConstructors().FirstOrDefault(x => x.GetParameters().Any(param => param.ParameterType == typeof(BotMiddlewareHandler)));
				if (constructor == null)
					throw new Exception("The constructor must accept a parameter of type NextExpressMiddleware");

				var constructorParameterInstances = constructor.GetParameters()
					.Select(
						x => x.ParameterType == typeof(BotMiddlewareHandler) ?
						CreateDelegate(prevMiddleware) :
						scope.ServiceProvider.GetService(x.ParameterType)
					);

				var instanse = Activator.CreateInstance(middleware.Class, constructorParameterInstances.ToArray());
				middleware.Instance = instanse;
				prevMiddleware = middleware;
			}
		}

		/// <summary>
		/// Запускает обработку входящего сообщения
		/// </summary>
		/// <param name="message"><see cref="UserMessage"/></param>
		/// <returns></returns>
		public async Task RunMiddlewareAsync(UserMessage message)
		{
			using var scope = serviceScopeFactory.CreateScope();
			var middleware = Middlewares.First();
			var parameters = middleware.Parameters.Select(x => x == typeof(UserMessage) ? message : scope.ServiceProvider.GetService(x)).ToArray();

			try
			{
				await middleware.Method.InvokeAsync(middleware.Instance, parameters);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error inside middleware");
				throw;
			}
		}

		private BotMiddlewareHandler CreateDelegate(MiddlewareData nextMiddleware)
		{
			if (nextMiddleware == null)
				return null;
			return async delegate (UserMessage message)
			{
				using var scope = serviceScopeFactory.CreateScope();
				var parameters = nextMiddleware.Parameters
					.Select(x => x == typeof(UserMessage) ? message : scope.ServiceProvider.GetService(x))
					.ToArray();

				await nextMiddleware.Method.InvokeAsync(nextMiddleware.Instance, parameters);
			};
		}
	}

	internal class MiddlewareData
	{
		/// <summary>
		/// Ссылка на класс миддлвари
		/// </summary>
		public Type Class { get; set; }
		/// <summary>
		/// Инстанс миддлвари, в котором вызывается метод <see cref="Method"/>
		/// </summary>
		public object Instance { get; set; }
		/// <summary>
		/// Ссылка на метод "Invoke" или "InvokeAsync", который необходимо вызвать
		/// </summary>
		public FastMethodInfo Method { get; set; }
		/// <summary>
		/// Список параметров для <see cref="Method"/>
		/// </summary>
		public List<Type> Parameters { get; set; }
	}
}
