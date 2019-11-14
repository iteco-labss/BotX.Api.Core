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
                machine.OnFinished += Machine_OnFinished;
                await machine.EnterAsync(userMessage);
                machine.Save(userMessage.From.Huid);
            }
            else
            {
                await MessageSender.ReplyTextMessageAsync(userMessage, "Use /start comand for start");
            }
        }

        private async void Machine_OnFinished(object sender, FinishedEventArgs e)
        {
            await MessageSender.ReplyTextMessageAsync(e.Message, $"State machine model is {JsonConvert.SerializeObject(e.Model)}");
            await MessageSender.ReplyTextMessageAsync(e.Message, "finished!");
        }
    }
}
