using BotX.Api.Abstract;
using BotX.Api.Delegates;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BotX.Api.Internal
{
	internal class ExecutableAction
	{
		internal string Name { get; set; }
		internal Type Type { get; set; }
		internal Dictionary<string, BotEventHandler> Commands { get; set; } = new Dictionary<string, BotEventHandler>();
	}
}
