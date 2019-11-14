using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class EnterNameState : BaseState
    {
        public override async Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
            model.Name = userMessage.Command.Body;
            await Stage.MessageSender.ReplyTextMessageAsync(userMessage, $"Nice to meet you, {model.Name}!");
            await Stage.TransitionToAsync(new EnterAgeState(), userMessage);
        }

        public override async Task WelcomeAsync(UserMessage userMessage, dynamic model)
        {
            await Stage.MessageSender.ReplyTextMessageAsync(userMessage, "What is your name?");           
        }
    }
}
