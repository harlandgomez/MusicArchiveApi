using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using MusicArchiveApi.Dtos;
using MusicArchiveApi.Interfaces;
using MusicArchiveApi.Models;
using Newtonsoft.Json;

namespace MusicArchiveApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicWikiController : ControllerBase
    {
        private readonly IMusicBrainAdapter _musicBrainAdapter;
        private readonly IWikidataAdapter _wikidataAdapter;
        private readonly IWikipediaAdapter _wikipediaAdapter;
        private readonly ICoverArtAdapter _coverArtAdapter;

        public MusicWikiController(IMusicBrainAdapter musicBrainAdapter,
            IWikidataAdapter wikidataAdapter,
            IWikipediaAdapter wikipediaAdapter,
            ICoverArtAdapter coverArtAdapter)
        {
            _musicBrainAdapter = musicBrainAdapter;
            _wikidataAdapter = wikidataAdapter;
            _wikipediaAdapter = wikipediaAdapter;
            _coverArtAdapter = coverArtAdapter;
        }

        // GET api/musicwiki/5
        [HttpGet("{mbId}")]
        [Produces("application/json")]
        public async Task<IActionResult> Get(string mbId)
        {
            var httpResponseMessage = await _musicBrainAdapter.GetMusicBrainzArtistByMbId(mbId);

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
                Albums = new List<Album>()
            };

            //In case relations is empty get description from Music Brainz Annotation
            if (artistResponse.Relations == null || 
                !artistResponse.Relations.Any() || 
                artistResponse.Relations.FirstOrDefault()?.Url == null
                )
            {
                musicBrainz.Description = await _musicBrainAdapter.GetMusicBrainzAnnotateDescriptionByMbId(mbId);
            }

            var wikiDataUrl = artistResponse.Relations.Where(t => t.Type == "wikidata").Select(u => u.Url)
                .FirstOrDefault()
                ?.Resource;
                
            var wikiDataQId = Regex.Match(wikiDataUrl, @"Q\d+").Value;
            var wikiTitle = await _wikidataAdapter.GetWikiDataTitleByQId(wikiDataQId);
            musicBrainz.Description = await _wikipediaAdapter.GetWikiDescriptionByTitle(HttpUtility.UrlPathEncode(wikiTitle));
            var httpClient = new HttpClient();
            await Task.Run(() =>
                {
                    Parallel.ForEach(artistResponse.ReleaseGroups, (releaseGroup) =>
                    {
                        var album = new Album
                        {
                            Id = releaseGroup.CoverArtId,
                            Title = releaseGroup.Title,
                            Image = _coverArtAdapter.GetCoverArtImageLinkByMbId(releaseGroup.CoverArtId).Result
                        };
                        musicBrainz.Albums.Add(album);
                    });
                }
            );

            return Ok(musicBrainz);
        }
    }
}
