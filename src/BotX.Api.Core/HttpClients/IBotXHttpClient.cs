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
		public Task SendFileAsync(Guid syncId, Guid botId, string fileName, byte[] data);
		public Task SendReplyAsync(ResponseMessage message);
		public Task SendNotificationAsync(NotificationMessage message);
	}
}
