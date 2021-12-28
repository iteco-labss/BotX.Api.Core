using BotX.Api.JsonModel.Api.Request;
using BotX.Api.JsonModel.Api.Response;
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
		public Task<FileMetadata> UploadFileAsync(Guid botId, Guid chatId, string fileName, byte[] data, string mimeType, FileMetaInfo meta);
		public Task<Guid> SendReplyAsync(Guid botId, ResponseMessage message);
		public Task EditMessageAsync(Guid botId, EditEventMessage message);
		public Task SendNotificationAsync(Guid botId, NotificationMessage message);
	}
}
