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
		public async Task SendFileAsync(UserMessage requestMessage, string fileName, byte[] data)
        {
            CheckFileSize(data);
            await httpClient.SendFileAsync(
				host: requestMessage.From.Host,
	            botId: requestMessage.BotId,
				syncId: requestMessage.SyncId,
				fileName: fileName,
	            data: data
            );
		}

		public async Task SendFileAsync(Uri cts, Guid botId, Guid chatId, Guid huid, string fileName, byte[] data)
		{
			CheckFileSize(data);
			await SendFileAsync(cts, botId, chatId, huid, null, fileName, data);
		}

		public async Task SendFileAsync(Uri cts, Guid botId, Guid chatId, Guid huid, string caption, string fileName, byte[] data)
		{
			CheckFileSize(data);
			var notification = new NotificationMessage
			{
				Recipients = new[] { huid },
				GroupChatId = chatId,
				File = new File
				{
					FileName = fileName,
					Caption = caption,
					Data = $"data:{GetMimeType(fileName)};base64,{Convert.ToBase64String(data)}"
				}
			};
			await httpClient.SendNotificationAsync(cts.ToString(), botId, notification);
		}

		private void CheckFileSize(byte[] fileData)
        {
            const int limitInMb = 100;
			if (fileData.Length > limitInMb * 1024 * 1024)
                throw new Exception($"The file size is too large. The maximum file size is limited to {limitInMb} MB");
        }

		private string GetMimeType(string fileName)
		{
			var provider = new FileExtensionContentTypeProvider();
			provider.TryGetContentType(fileName, out string mimeType);
			return mimeType ?? "application/octet-stream";
		}
	}
}
