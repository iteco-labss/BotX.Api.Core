using Newtonsoft.Json;

namespace BotX.Api.JsonModel.Api.Request
{
    public class FileMetaInfo
    {
        [JsonProperty(PropertyName = "duration")]
        public int? Duration { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }
    }
}
