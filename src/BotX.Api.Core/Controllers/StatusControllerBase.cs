using BotX.Api.JsonModel.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.Controllers
{
	[Route("/status")]
	[ApiController]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class StatusController : ControllerBase
	{
		private readonly ILogger<StatusController> logger;

		public StatusController(ILogger<StatusController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public IActionResult Get()
		{
			var response = new ResponseStatus
			{
				Status = "ok",
				Result = new ResponseResult
				{
					Enabled = true,
					Status_message = "hi",
					Commands = ExpressBotService.commands.ToArray()
				}
			};

			logger.LogInformation("return botx status");

			return Ok(response);
		}		
	}
}
