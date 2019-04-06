using System.Net.Http;
using System.Threading.Tasks;

namespace MusicArchiveApi.Interfaces
{
    public interface IMusicBrainzService
    {
        Task<HttpResponseMessage> GetMusicBrainzArtistByMbId(string mbId, HttpClient httpClient);
    }
}