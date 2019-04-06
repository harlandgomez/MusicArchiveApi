using Newtonsoft.Json;

namespace MusicArchiveApi.Dtos
{
    public class MbUrl
    {
        [JsonProperty("resource")] public string Resource { get; set; }
    }
}