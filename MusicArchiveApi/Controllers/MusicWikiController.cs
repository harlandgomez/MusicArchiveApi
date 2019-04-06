using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using MusicArchiveApi.Configuration;
using MusicArchiveApi.Dtos;
using MusicArchiveApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MusicArchiveApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicWikiController : ControllerBase
    {
        private readonly BaseUrlSettings _urlSettings;

        public MusicWikiController(IOptions<BaseUrlSettings> urlSettings)
        {
            _urlSettings = urlSettings.Value;
        }

        // GET api/musicwiki/5
        [HttpGet("{mbId}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string mbId)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");

            var httpResponseMessage = await GetMusicBrainzArtistByMbId(mbId, httpClient);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return BadRequest("Invalid request: MBId does not exist");
            }

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            var artistResponse = JsonConvert.DeserializeObject<MbArtistResponse>(json);


            if (artistResponse.ReleaseGroups == null || !artistResponse.ReleaseGroups.Any())
            {
                return NotFound("Artist not found");
            }

            var musicBrainz = new MusicBrainz
            {
                MbId = mbId,
                Album = new List<Album>()
            };

            //In case relations is empty
            if (artistResponse.Relations == null || 
                !artistResponse.Relations.Any() || 
                artistResponse.Relations.FirstOrDefault()?.Url == null
                )
            {
                musicBrainz.Description = await GetMusicBrainzAnnotateDescriptionByMbId(mbId, httpClient);
            }

            var wikiDataUrl = artistResponse.Relations.Where(t => t.Type == "wikidata").Select(u => u.Url)
                .FirstOrDefault()
                ?.Resource;
                
            var wikiDataQId = Regex.Match(wikiDataUrl, @"Q\d+").Value;
            var wikiTitle = await GetWikiDataTitleByQId(wikiDataQId);
            musicBrainz.Description = await GetWikiDescriptionByTitle(HttpUtility.UrlPathEncode(wikiTitle));


            foreach (var releaseGroup in artistResponse.ReleaseGroups)
            {
                var album = new Album
                {
                    Id = releaseGroup.CoverArtId,
                    Title = releaseGroup.Title,
                    Image = "test"
                };
                musicBrainz.Album.Add(album);
            }

            return Ok(musicBrainz);
        }

        private async Task<string> GetWikiDataTitleByQId(string qId)
        {
            var httpClient = new HttpClient();
            var title = string.Empty;
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");

            var wikiTitleQuery = new UriBuilder(_urlSettings.WikidataUrl)
            {
                Query = $"action=wbgetentities&ids={qId}&format=json&props=sitelinks",
                Port = -1
            };

            try
            {
                var httpResponseMessage = httpClient.GetAsync(wikiTitleQuery.Uri).Result;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    dynamic wikiDataResponse = JsonConvert.DeserializeObject(json);
                    title = wikiDataResponse.entities[qId].sitelinks["enwiki"].title;
                }

            }
            catch (Exception ex)
            {
                return $"error:{ex}" ;
            }
            return title;
        }

        private async Task<string> GetMusicBrainzAnnotateDescriptionByMbId(string mbId, HttpClient httpClient)
        {
            var annotateQuery = new UriBuilder(_urlSettings.MusicBrainzUrl + "annotation/")
            {
                Query = $"query=entity:{mbId}&fmt=json",
                Scheme = "http",
                Port = -1
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, annotateQuery.Uri);
            var httpResponseMessage = await httpClient.SendAsync(requestMessage);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic annotateResponse = JsonConvert.DeserializeObject(json);
            return annotateResponse.annotations[0].text;
        }

        private async Task<string> GetWikiDescriptionByTitle(string title)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)");

            var mbArtistQuery = new UriBuilder(_urlSettings.WikipediaUrl)
            {
                Query = "action=query&format=json&prop=extracts&exintro=true&redirects=true" +
                        $"&titles={title}",
                Port = -1
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, mbArtistQuery.Uri);
            var httpResponseMessage = await httpClient.SendAsync(requestMessage);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var json = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic wikiResponse = JsonConvert.DeserializeObject(json);
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

        private async Task<HttpResponseMessage> GetMusicBrainzArtistByMbId(string mbId, HttpClient httpClient)
        {
            var mbArtistQuery = new UriBuilder(_urlSettings.MusicBrainzUrl + $"artist/{mbId}")
            {
                Query = "fmt=json&inc=url-rels+release-groups",
                Scheme = "http",
                Port = -1
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, mbArtistQuery.Uri);
            var httpResponseMessage = await httpClient.SendAsync(requestMessage);
            return httpResponseMessage;
        }
    }
}
