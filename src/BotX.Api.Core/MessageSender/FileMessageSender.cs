using BotX.Api.JsonModel.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
	}
}
