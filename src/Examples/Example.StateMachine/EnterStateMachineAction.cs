using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    [BotAction]
    public class EnterStateMachineAction : BotAction
    {
        public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
        {
            var machine = DemoStateMachine.Get(userMessage.From.Huid, MessageSender);
            if (machine != null)
            {
                await machine.EnterAsync(userMessage);
                machine.Save(userMessage.From.Huid);
            }
            else
            {
                await MessageSender.ReplyTextMessageAsync(userMessage, "Use /start command for start");
            }
        }
    }
}
