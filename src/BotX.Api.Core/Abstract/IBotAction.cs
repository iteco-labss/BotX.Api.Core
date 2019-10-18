using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Abstract
{
	
	internal interface IBotAction
	{		
		Task ExecuteAsync(UserMessage userMessage, string[] args);
		Task OnChatCreated(UserMessage userMessage);
	}

}
