using BotX.Api.Attributes;
using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.Abstract
{	
	internal interface IBotAction
	{		
		Task ExecuteAsync(UserMessage userMessage, string[] args);
	}

}
