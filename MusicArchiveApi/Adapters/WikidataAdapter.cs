using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;
using Newtonsoft.Json;

namespace MusicArchiveApi.Adapters
{
    public class WikidataAdapter: BaseAdapter, IWikidataAdapter
    {
        public WikidataAdapter(IOptions<BaseUrlSettings> urlSetting) : base(urlSetting)
        {
        }

        public async Task<string> GetWikiDataTitleByQId(string qId)
        {
            var wikiTitleQuery = new UriBuilder(UrlSettings.WikidataUrl)
            {
                Query = $"action=wbgetentities&ids={qId}&format=json&props=sitelinks",
                Port = NoPort
            };

            var httpResponseMessage = HttpClient.GetAsync(wikiTitleQuery.Uri).Result;
            if (!httpResponseMessage.IsSuccessStatusCode) return "Missing Title";
            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic wikiDataResponse = JsonConvert.DeserializeObject(json);
            return wikiDataResponse.entities[qId].sitelinks["enwiki"].title;
        }

    }
}