using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class ResponseStatus
	{
		public string Status { get; set; }

		public ResponseResult Result { get; set; }
	}

	public class ResponseResult
	{
		public bool Enabled { get; set; }

		public string Status_message { get; set; }

		public Command[] Commands { get; set; }
	}
}
