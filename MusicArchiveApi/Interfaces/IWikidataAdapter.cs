using System.Threading.Tasks;

namespace MusicArchiveApi.Interfaces
{
    public interface IWikidataAdapter
    {
        Task<string> GetWikiDataTitleByQId(string qId);
    }
}