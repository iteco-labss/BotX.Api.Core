using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    public class EnterAgeState : BaseState
    {
		public override async Task ExecuteAsync(UserMessage userMessage, dynamic model)
        {
            if (!int.TryParse(userMessage.Command.Body, out int age))
            {
                await Stage.MessageSender.ReplyTextMessageAsync(userMessage, "Please enter the number");
                await Stage.TransitionToAsync(new EnterAgeState());
            }
            else
            {
                model.Age = age;
                await Stage.TransitionToAsync(new ResultState());
            }
        }

        public override async Task WelcomeAsync(UserMessage userMessage, dynamic model)
        {
            await Stage.MessageSender.ReplyTextMessageAsync(userMessage, "How old are you?");
        }
    }
}
