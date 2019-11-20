using BotX.Api;
using BotX.Api.StateMachine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
	public class DemoStateMachine : BaseStateMachine
    {
        private static Dictionary<Guid, string> userStates = new Dictionary<Guid, string>();

		public DemoStateMachine(BotMessageSender messageSender) : base(messageSender)
		{
		}

        public override async Task OnFinishedAsync(dynamic model)
        {
            await MessageSender.ReplyTextMessageAsync(UserMessage, $"State machine model is {JsonConvert.SerializeObject(model)}");
			await MessageSender.ReplyTextMessageAsync(UserMessage, "finished!");
			userStates.Remove(UserMessage.From.Huid);
        }

		public override async Task OnStartedAsync()
		{
			await TransitionToAsync<EnterNameState>();
		}

		public override BaseStateMachine RestoreState()
		{
			if (userStates.ContainsKey(UserMessage.From.Huid))
				return FromJson(userStates[UserMessage.From.Huid], MessageSender);
			return null;
		}

		public override void SaveState()
		{
			userStates[UserMessage.From.Huid] = this.ToJson();
		}
	}
}
