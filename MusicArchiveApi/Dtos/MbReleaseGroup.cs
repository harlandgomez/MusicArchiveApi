using Newtonsoft.Json;

namespace MusicArchiveApi.Dtos
{
    public class MbReleaseGroup
    {
        [JsonProperty("id")] public string CoverArtId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
    }
}