using BotX.Api;
using BotX.Api.StateMachine;
using Example.StateMachine.States;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.StateMachine
{
	public class DemoStateMachine : BaseStateMachine
	{
		private static Dictionary<Guid, string> statesStorage = new Dictionary<Guid, string>();

		public DemoStateMachine(IBotMessageSender messageSender, IServiceScopeFactory scopeFactory) : base(messageSender, scopeFactory)
		{
		}

		public override async Task OnFinishedAsync()
		{
			await MessageSender.ReplyTextMessageAsync(UserMessage, "Thank you!!");
			await MessageSender.ReplyTextMessageAsync(UserMessage, $"result: {JsonConvert.SerializeObject(Model)}");
		}

		public override async Task OnStartedAsync()
		{
			await TransitionToAsync<GenderState>();
		}

		public override BaseStateMachine RestoreState()
		{
			if (statesStorage.ContainsKey(UserMessage.From.Huid))
				return FromJson<DemoStateMachine>(statesStorage[UserMessage.From.Huid], MessageSender, UserMessage);
			return null;	
		}

		public override void SaveState()
		{
			statesStorage[UserMessage.From.Huid] = ToJson();
		}
	}
}
