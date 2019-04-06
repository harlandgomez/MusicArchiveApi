using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Interfaces;
using Newtonsoft.Json;

namespace MusicArchiveApi.Adapters
{
    public class WikipediaAdapter: BaseAdapter, IWikipediaAdapter
    {
        public WikipediaAdapter(IOptions<BaseUrlSettings> urlSetting) : base(urlSetting)
        {
        }

        public async Task<string> GetWikiDescriptionByTitle(string title)
        {
            var mbArtistQuery = new UriBuilder(UrlSettings.WikipediaUrl)
            {
                Query = "action=query&format=json&prop=extracts&exintro=true&redirects=true" +
                        $"&titles={title}",
                Port = NoPort
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, mbArtistQuery.Uri);
            var httpResponseMessage = await HttpClient.SendAsync(requestMessage);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            return ParseDescription(json);
        }

        /// <summary>
        /// Search thru the extract node in a json string
        /// </summary>
        /// <param name="json">The json string</param>
        /// <returns>Description of the Artist</returns>
        private static string ParseDescription(string json)
        {
            var description = string.Empty;
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                var isDescFound = false;
                var isFinish = false;
                while (reader.Read() && !isFinish)
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Value.ToString() == "extract")
                    {
                        isDescFound = true;
                    }

                    if (!isDescFound || reader.TokenType != JsonToken.String) continue;
                    description = reader.Value.ToString();
                    isFinish = true;
                }
            }

            return description;
        }
    }
}