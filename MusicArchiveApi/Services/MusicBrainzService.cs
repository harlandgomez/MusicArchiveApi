using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MusicArchiveApi.Dtos;
using MusicArchiveApi.Exceptions;
using MusicArchiveApi.Interfaces;
using MusicArchiveApi.Models;
using Newtonsoft.Json;

namespace MusicArchiveApi.Services
{
    public class MusicBrainzService : IMusicBrainzService
    {
        private readonly IMusicBrainAdapter _musicBrainAdapter;
        private readonly IWikidataAdapter _wikidataAdapter;
        private readonly IWikipediaAdapter _wikipediaAdapter;
        private readonly ICoverArtAdapter _coverArtAdapter;
        private const string WikidataType = "wikidata";
        private const string QIdPattern = @"Q\d+";

        public MusicBrainzService(IMusicBrainAdapter musicBrainAdapter, 
            IWikidataAdapter wikidataAdapter,
            IWikipediaAdapter wikipediaAdapter,
            ICoverArtAdapter coverArtAdapter)
        {
            _musicBrainAdapter = musicBrainAdapter;
            _wikidataAdapter = wikidataAdapter;
            _wikipediaAdapter = wikipediaAdapter;
            _coverArtAdapter = coverArtAdapter;
        }

        public async Task<MusicBrainz> GetMusicBrainz(string mbId)
        {
            var httpResponseMessage = await _musicBrainAdapter.GetMusicBrainzArtistByMbId(mbId);

            var json = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new BadRequestException("Invalid request: MBId does not exist");
            }

            var artistResponse = JsonConvert.DeserializeObject<MbArtistResponse>(json);

            if (artistResponse.ReleaseGroups == null || !artistResponse.ReleaseGroups.Any())
            {
                throw new NotFoundException("Not Found request: Artist");
            }

            var relations = artistResponse.Relations;

            var musicBrainz = new MusicBrainz
            {
                MbId = mbId,
                Description = await GetDescriptionIfUrlIsEmpty(relations.FirstOrDefault()?.Url, mbId),
                Albums = new List<Album>()
            };

            var wikiDataQId = GetWikidataQId(relations);
            var wikiTitle = await _wikidataAdapter.GetWikiDataTitleByQId(wikiDataQId);
            musicBrainz.Description = await _wikipediaAdapter.GetWikiDescriptionByTitle(HttpUtility.UrlPathEncode(wikiTitle));

            await Task.Run(() =>
                {
                    Parallel.ForEach(artistResponse.ReleaseGroups, (releaseGroup) =>
                    {
                        var album = new Album
                        {
                            Id = releaseGroup.CoverArtId,
                            Title = releaseGroup.Title,
                            Image = _coverArtAdapter
                                    .GetCoverArtImageLinkByMbId(releaseGroup.CoverArtId)
                                    .Result
                        };
                        musicBrainz.Albums.Add(album);
                    });
                }
            );

            return musicBrainz;
        }

        private static string GetWikidataQId(IEnumerable<MbRelation> relations)
        {
            var wikiDataUrl = relations.Where(t => t.Type == WikidataType)
                .Select(u => u.Url)
                .FirstOrDefault()
                ?.Resource;

            return Regex.Match(wikiDataUrl 
                    ?? throw new InvalidOperationException(), QIdPattern).Value;
        }

        private async Task<string> GetDescriptionIfUrlIsEmpty(MbUrl url, string mbId)
        {
            return url == null 
                ? await _musicBrainAdapter.GetMusicBrainzAnnotateDescriptionByMbId(mbId) 
                : string.Empty;
        }
        
    }
}
