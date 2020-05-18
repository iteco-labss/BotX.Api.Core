using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotX.Api.HttpClients
{
	internal interface IBotXHttpClient
	{
		 Task SendFileAsync(Guid syncId, Guid botId, string fileName, byte[] data);
		 Task<Guid> SendReplyAsync(ResponseMessage message);
		 Task EditMessageAsync(EditEventMessage message);
		 Task SendNotificationAsync(NotificationMessage message);
	}
}
