﻿using BotX.Api.JsonModel.Response.Mentions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
    public class Mention
    {
        [JsonProperty("mention_id")]
	    public Guid Id { get; set; }

        [JsonProperty("mention_data")]
        public MentionData MentionData { get; set; }

        [JsonProperty("mention_type")]
        public string MentionType = "contact";

    }
}
