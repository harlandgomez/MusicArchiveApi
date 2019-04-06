namespace MusicArchiveApi.Models
{
    /// <summary>
    /// Album contains:
    /// 1. Title (title of the album)
    /// 2. Id (Id per Entity from Music Brainz i.e.:
    ///     area, artist, event, instrument,
    ///     label, place, recording, release,
    ///     release-group, series, work, url
    /// 3. Image (Image url from CoverArt) 
    /// </summary>
    public class Album  
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
    }
}