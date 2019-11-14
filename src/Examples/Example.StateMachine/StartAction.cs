using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using Example.ChatProcessing.Bot.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Example.StateMachine
{
    [BotAction("start")]
    public class StartAction : BotAction
    {
        public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
        {
            await MessageSender.ReplyTextMessageAsync(userMessage, "Hello!");
            var machine = new DemoStateMachine(new EnterNameState(), MessageSender);
            await machine.EnterAsync(userMessage);
            machine.Save(userMessage.From.Huid);
        }
    }
}
