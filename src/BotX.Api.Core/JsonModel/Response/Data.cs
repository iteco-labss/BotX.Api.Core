﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
	public class Data
	{
		[JsonProperty("eventtype")]
		public string EventType { get; set; }
		[JsonProperty("payload")]
		public string Payload { get; set; }
	}
}
