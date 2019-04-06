using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;
using Newtonsoft.Json;

namespace MusicArchiveApi.Adapters 
{
    public class CoverArtAdapter : BaseAdapter, ICoverArtAdapter
    {
        public CoverArtAdapter(IOptions<BaseUrlSettings> urlSetting) : base(urlSetting)
        {
        }

        public async Task<string> GetCoverArtImageLinkByMbId(string mbId)
        {
            var coverArtQuery = new UriBuilder(UrlSettings.CoverArtUrl + $"release-group/{mbId}")
            {
                Scheme = "http",
                Port = NoPort
            };

            var httpResponseMessage = await HttpClient.GetAsync(coverArtQuery.Uri);
            if (!httpResponseMessage.IsSuccessStatusCode) return "Missing cover art image link";
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic covertArtResponse = JsonConvert.DeserializeObject(json);
            return covertArtResponse.images.First.image;

        }
    }
}
