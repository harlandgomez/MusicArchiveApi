using System.Threading.Tasks;

namespace MusicArchiveApi.Interfaces
{
    public interface ICoverArtAdapter
    {
        Task<string> GetCoverArtImageLinkByMbId(string mbId);
    }
}