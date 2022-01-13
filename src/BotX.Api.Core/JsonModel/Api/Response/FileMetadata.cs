using Newtonsoft.Json;
using System;

namespace BotX.Api.JsonModel.Api.Response
{
#pragma warning disable CS1591
    public class FileMetadataResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public FileMetadata Result { get; set; }
    }

    public class FileMetadata
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("file_mime_type")]
        public string FileMimeType { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_preview")]
        public string FilePreview { get; set; }

        [JsonProperty("file_preview_height")]
        public int? FilePreviewHeight { get; set; }

        [JsonProperty("file_preview_width")]
        public int? FilePreviewWidth { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }

        [JsonProperty("file_hash")]
        public string FileHash { get; set; }

        [JsonProperty("file_encryption_algo")]
        public string FileEncriptionAlgo { get; set; }

        [JsonProperty("chunk_size")]
        public int ChunkSize { get; set; }

        [JsonProperty("file_id")]
        public Guid FileId { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonProperty("duration")]
        public int? Duration { get; set; }

    }
}
