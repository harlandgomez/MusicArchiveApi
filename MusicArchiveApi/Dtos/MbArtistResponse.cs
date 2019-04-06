using System.Collections.Generic;
using Newtonsoft.Json;

namespace MusicArchiveApi.Dtos
{
    public class MbArtistResponse
    {
        [JsonProperty("relations")] public List<MbRelation> Relations { get; set; } 
        [JsonProperty("release-groups")] public List<MbReleaseGroup> ReleaseGroups { get; set; }
    }
}