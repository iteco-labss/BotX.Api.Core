using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class EnterAgeState : BaseQuestionState
    {
		public override async Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
            if (!int.TryParse(userMessage.Command.Body, out int age))
            {
                await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, "Please enter the number");
                await StateMachine.TransitionToAsync<EnterAgeState>();
            }
            else
            {
                model.Age = age;
                await StateMachine.TransitionToAsync<ResultState>();
            }
        }

        public override async Task WelcomeAsync(UserMessage userMessage, dynamic model)
        {
            await StateMachine.MessageSender.ReplyTextMessageAsync(userMessage, "How old are you?");
        }
    }
}
