using BotX.Api;
using BotX.Api.Abstract;
using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using BotX.Api.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example.ChatProcessing.Bot.StateMachine
{
    [BotAction]
    public class HelpAction : BotAction
    {
		public HelpAction(IBotMessageSender messageSender) : base(messageSender)
		{
		}

		public override async Task ExecuteAsync(UserMessage userMessage, string[] args)
        {
            await MessageSender.ReplyTextMessageAsync(userMessage, "Use /start command for start");
        }
    }
}
