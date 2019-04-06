using System.Threading.Tasks;

namespace MusicArchiveApi.Interfaces
{
    public interface IWikipediaAdapter
    {
        Task<string> GetWikiDescriptionByTitle(string title);
    }
}