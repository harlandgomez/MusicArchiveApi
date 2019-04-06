using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;
using Newtonsoft.Json;

namespace MusicArchiveApi.Adapters
{
    public class MusicBrainzAdapter : BaseAdapter, IMusicBrainAdapter
    {
        public MusicBrainzAdapter(IOptions<BaseUrlSettings> urlSetting) : base(urlSetting)
        {
        }

        public async Task<HttpResponseMessage> GetMusicBrainzArtistByMbId(string mbId)
        {
            var mbArtistQuery = new UriBuilder(UrlSettings.MusicBrainzUrl + $"artist/{mbId}")
            {
                Query = "fmt=json&inc=url-rels+release-groups",
                Scheme = "http",
                Port = NoPort
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, mbArtistQuery.Uri);
            var httpResponseMessage = await HttpClient.SendAsync(requestMessage);
            return httpResponseMessage;
        }

        public async Task<string> GetMusicBrainzAnnotateDescriptionByMbId(string mbId)
        {
            var annotateQuery = new UriBuilder(UrlSettings.MusicBrainzUrl + "annotation/")
            {
                Query = $"query=entity:{mbId}&fmt=json",
                Scheme = "http",
                Port = -1
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, annotateQuery.Uri);
            var httpResponseMessage = await HttpClient.SendAsync(requestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic annotateResponse = JsonConvert.DeserializeObject(json);
            return annotateResponse.annotations[0].text;
        }
    }
}
