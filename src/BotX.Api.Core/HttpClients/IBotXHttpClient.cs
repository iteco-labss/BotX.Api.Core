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
		public Task SendFileAsync(string host, Guid botId, Guid syncId, string fileName, byte[] data);
		public Task<Guid> SendReplyAsync(string host, Guid botId, ResponseMessage message);
		public Task EditMessageAsync(string host, Guid botId, EditEventMessage message);
		public Task SendNotificationAsync(string host, Guid botId, NotificationMessage message);
	}
}
