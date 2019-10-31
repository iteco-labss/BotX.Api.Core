using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel
{
    internal interface IMessage
    {
        public Guid BotId { get; set; }
    }
}
