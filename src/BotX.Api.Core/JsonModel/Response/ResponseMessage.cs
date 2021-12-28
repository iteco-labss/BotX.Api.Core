using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
    public class ResponseMessage
    {
		[JsonProperty(PropertyName = "recipients", NullValueHandling = NullValueHandling.Ignore)]
        public Guid[] Recipients { get; set; }

        [JsonProperty(PropertyName = "group_chat_id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid GroupChatId { get; set; }

        [JsonProperty("notification")]
        public CommandResult Notification { get; set; }

        [JsonProperty("file")]
        public File File { get; set; }

	}
}
