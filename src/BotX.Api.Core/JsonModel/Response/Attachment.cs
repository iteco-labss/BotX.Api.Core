using Newtonsoft.Json;

namespace BotX.Api.JsonModel.Response
{
#pragma warning disable CS1591
    public class Attachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public AttachmentData Data { get; set; }
	}

    public class AttachmentData
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

        [JsonProperty("location_name")]
        public string LocationName { get; set; }

        [JsonProperty("location_address")]
        public string LocationAddress { get; set; }

        [JsonProperty("location_lat")]
        public decimal? LocationLat { get; set; }

        [JsonProperty("location_lng")]
        public decimal? LocationLng { get; set; }

        [JsonProperty("contact_name")]
        public string ContactName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("url_title")]
        public string UrlTitle { get; set; }

        [JsonProperty("url_preview")]
        public string UrlPreview { get; set; }

        [JsonProperty("url_text")]
        public string UrlText { get; set; }
    }
}