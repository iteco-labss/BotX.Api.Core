using BotX.Api.Abstract;
using BotX.Api.BotUI;
using BotX.Api.Extensions;
using BotX.Api.JsonModel.Request;
using BotX.Api.JsonModel.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using BotX.Api.JsonModel;
using BotX.Api.JsonModel.Response.Mentions;
using BotX.Api.HttpClients;

namespace BotX.Api
{
	internal partial class BotMessageSender : IBotMessageSender
	{
		
		private readonly ILogger<BotMessageSender> logger;
		private readonly IBotXHttpClient httpClient;

		/// <summary>
		/// Инициализация нового экземпляра клиента
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="httpClient"></param>
		internal BotMessageSender(ILogger<BotMessageSender> logger, IBotXHttpClient httpClient)
		{
			this.logger = logger;
			this.httpClient = httpClient;
		}
	}
}
