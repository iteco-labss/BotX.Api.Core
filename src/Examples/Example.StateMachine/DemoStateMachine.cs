using BotX.Api;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class DemoStateMachine : BaseStateMachine
    {
        private static Dictionary<Guid, string> userStates = new Dictionary<Guid, string>();

        public DemoStateMachine(BaseState initialState, BotMessageSender messageSender) : base(initialState, messageSender)
        {
        }

        internal static bool IsMachineStarted(Guid huid)
        {
            return userStates.ContainsKey(huid);
        }

        internal void Save(Guid huid)
        {
            userStates[huid] = ToJson();
        }

        internal static DemoStateMachine Get(Guid huid, BotMessageSender messageSender)
        {
            if (userStates.ContainsKey(huid))
                return FromJson(userStates[huid], messageSender) as DemoStateMachine;
            return null;
        }
    }
}
