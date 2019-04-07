using System.Threading.Tasks;
using MusicArchiveApi.Models;

namespace MusicArchiveApi.Interfaces
{
    public interface IMusicBrainzService
    {
        Task<MusicBrainz> GetMusicBrainz(string mbId);
    }
}