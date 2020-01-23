using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel
{
    // TODO обсудить с андреем зачем нам IMessage
    // сейчас BotId нужен только для нотификации
    internal interface IMessage
    {
        public Guid BotId { get; set; }
    }
}
