using System.Net.Http;
using System.Threading.Tasks;

namespace MusicArchiveApi.Interfaces
{
    public interface IMusicBrainAdapter
    {
        Task<HttpResponseMessage> GetMusicBrainzArtistByMbId(string mbId);
        Task<string> GetMusicBrainzAnnotateDescriptionByMbId(string mbId);
    }
}