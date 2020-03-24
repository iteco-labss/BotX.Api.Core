using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotX.Api.JsonModel.Response.Mentions
{
    public class MentionData
    {
        [JsonProperty("user_huid")]
        public Guid Huid { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }

        // TODO возможно стоит сделать перечисление?
        [JsonProperty("conn_type")]
        public string ConnectionType { get; set; }
    }
}
