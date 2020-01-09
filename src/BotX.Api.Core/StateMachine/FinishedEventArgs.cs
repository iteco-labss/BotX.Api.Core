using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.StateMachine
{
    public class FinishedEventArgs
    {
        public dynamic Model { get; set; }

        public UserMessage Message { get; set; }
    }
}
