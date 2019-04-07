using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicArchiveApi.Interfaces;

namespace MusicArchiveApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MusicWikiController : ControllerBase
    {
        private readonly IMusicBrainzService _musicBrainzService;

        public MusicWikiController(IMusicBrainzService musicBrainzService)
        {
            _musicBrainzService = musicBrainzService;
        }

        /// <summary>
        /// Passing the mbId as routing element
        /// GET api/musicwiki/[mbId]
        /// </summary>
        /// <param name="mbId"><see cref="mbId"/></param>
        /// <returns>json format of Music Brainz info with album details</returns>
        [HttpGet("{mbId}")]
        public async Task<IActionResult> Get(string mbId)
        {
            var musicBrainz = await _musicBrainzService.GetMusicBrainz(mbId);
            return Ok(musicBrainz);
        }
    }
}
