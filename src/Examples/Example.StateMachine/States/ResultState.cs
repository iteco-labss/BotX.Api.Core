using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class ResultState : BaseState
    {
        public override Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
            throw new NotImplementedException();
        }

        public override async Task WelcomeAsync(UserMessage userMessage, dynamic model)
        {
            await Stage.MessageSender.ReplyTextMessageAsync(userMessage, $"Thanks!");
            Stage.Finish();
        }
    }
}
