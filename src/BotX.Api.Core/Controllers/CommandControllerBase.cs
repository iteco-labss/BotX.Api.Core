using BotX.Api.JsonModel.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BotX.Api.Controllers
{
	[Route("/command")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class CommandControllerBase : ControllerBase
	{
		private readonly ActionExecutor actionExecutor;
		private readonly ILogger<CommandControllerBase> logger;
		private readonly BotMessageSender sender;

		public CommandControllerBase(ActionExecutor actionExecutor, ILogger<CommandControllerBase> logger, BotMessageSender sender)
		{
			this.actionExecutor = actionExecutor;
			this.logger = logger;
			this.sender = sender;
		}

		[HttpPost]
		public IActionResult Post([FromBody] UserMessage message)
		{
			logger.LogInformation("message has been received");
			Task.Run(async () => 
			{
				try
				{
					await actionExecutor.ExecuteAsync(message);
				}
				catch(Exception ex)
				{
					await sender.SendTextMessageAsync(message, ex.ToString());
					throw;
				}
			});

			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			return Ok("botx");
		}
	}
}
