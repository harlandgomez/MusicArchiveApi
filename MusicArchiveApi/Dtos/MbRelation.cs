using Newtonsoft.Json;

namespace MusicArchiveApi.Dtos
{
    public class MbRelation
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")] public MbUrl Url { get; set; }
    }
}