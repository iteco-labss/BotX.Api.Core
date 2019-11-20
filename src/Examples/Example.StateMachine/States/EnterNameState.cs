using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class EnterNameState : BaseQuestionState
    {
        public override async Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
            model.Name = userMessage.Command.Body;
            await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, $"Nice to meet you, {model.Name}!");
            await StateMachine.TransitionToAsync<EnterAgeState>();
        }

        public override async Task WelcomeAsync(UserMessage userMessage, dynamic model)
        {
            await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, "What is your name?");           
        }
    }
}
