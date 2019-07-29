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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class CommandControllerBase : ControllerBase
	{
		private readonly ActionExecutor actionExecutor;
		private readonly ILogger<CommandControllerBase> logger;

		public CommandControllerBase(ActionExecutor actionExecutor, ILogger<CommandControllerBase> logger)
		{
			this.actionExecutor = actionExecutor;
			this.logger = logger;
		}

		[HttpPost]
		public IActionResult Post([FromBody] UserMessage message)
		{
			logger.LogInformation("message has been received");
			Task.Run(async () => 
			{
				await actionExecutor.ExecuteAsync(message);
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
