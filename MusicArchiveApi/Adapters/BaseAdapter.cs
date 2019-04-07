using System.Net.Http;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;

namespace MusicArchiveApi.Adapters
{
    public abstract class BaseAdapter
    {
        protected readonly BaseUrlSettings UrlSettings;
        protected readonly HttpClient HttpClient;
        protected const int NoPort = -1;

        protected BaseAdapter(IOptions<BaseUrlSettings> urlSetting)
        {
            UrlSettings = urlSetting.Value;
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");
        }
    }
}
