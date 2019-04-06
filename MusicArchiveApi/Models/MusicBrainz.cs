using System.Collections.Generic;

namespace MusicArchiveApi.Models
{
    /// <summary>
    /// MusicBrainz model contains:
    /// 1. MbId (Music Brainz Id)
    /// 2. Description (Music Brainz/Wikipedia/Wikidata description)
    /// 2. Album <see cref="Album"/>
    /// </summary>
    public class MusicBrainz
    {
        public string MbId { get; set; }
        public string Description { get; set; }
        public List<Album> Album { get; set; }
    }
}