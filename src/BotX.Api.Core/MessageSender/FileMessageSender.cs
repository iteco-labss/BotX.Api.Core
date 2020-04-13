using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BotX.Api.JsonModel.Response;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;

namespace BotX.Api
{
	internal partial class BotMessageSender : IBotMessageSender
	{
		public async Task SendFileAsync(Guid syncId, string fileName, byte[] data)
		{
			await httpClient.SendFileAsync(
				syncId: syncId,
				botId: ExpressBotService.Configuration.BotId,
				fileName: fileName,
				data: data
				);
		}

		public async Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data)
		{
			await SendFileAsync(
				syncId: requestMessage.SyncId,
				fileName: fileName,
				data: data
				);
		}

		public async Task SendFileAsync(Guid chatId, Guid huid, string fileName, byte[] data)
		{
			await SendFileAsync(chatId, huid, null, fileName, data);
		}

		public async Task SendFileAsync(Guid chatId, Guid huid, string caption, string fileName, byte[] data)
		{
			var notification = new NotificationMessage
			{
				Recipients = new[] { huid },
				GroupChatIds = new[] { chatId },
				File = new File
				{
					FileName = fileName,
					Caption = caption,
					Data = $"data:{GetMimeType(fileName)};base64,{Convert.ToBase64String(data)}"
				}
			};
			await httpClient.SendNotificationAsync(notification);
		}

		private string GetMimeType(string fileName)
		{
			var provider = new FileExtensionContentTypeProvider();
			provider.TryGetContentType(fileName, out string mimeType);
			return mimeType ?? "application/octet-stream";
		}
	}
}
