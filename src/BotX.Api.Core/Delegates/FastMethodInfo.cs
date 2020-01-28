using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Delegates
{
	internal class FastMethodInfo
	{
		private delegate Task ReturnValueDelegate(object instance, object[] arguments);
		public FastMethodInfo(MethodInfo methodInfo)
		{
			var instanceExpression = Expression.Parameter(typeof(object), "instance");
			var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
			var argumentExpressions = new List<Expression>();
			var parameterInfos = methodInfo.GetParameters();
			for (var i = 0; i < parameterInfos.Length; ++i)
			{
				var parameterInfo = parameterInfos[i];
				argumentExpressions.Add(Expression.Convert(Expression.ArrayIndex(argumentsExpression, Expression.Constant(i)), parameterInfo.ParameterType));
			}
			var callExpression = Expression.Call(Expression.Convert(instanceExpression, methodInfo.ReflectedType), methodInfo, argumentExpressions);
			Delegate = Expression.Lambda<ReturnValueDelegate>(Expression.Convert(callExpression, typeof(Task)), instanceExpression, argumentsExpression).Compile();
		}

		private ReturnValueDelegate Delegate { get; }

		public async Task InvokeAsync(object instance, params object[] arguments)
		{
			await Delegate(instance, arguments);
		}
	}
}
