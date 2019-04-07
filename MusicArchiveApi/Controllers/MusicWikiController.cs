using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicArchiveApi.Exceptions;
using MusicArchiveApi.Interfaces;
using MusicArchiveApi.Models;

namespace MusicArchiveApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Controller for searching artist via Music Brainz Id (MbId)
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MusicWikiController : ControllerBase
    {
        private readonly IMusicBrainzService _musicBrainzService;

        /// <summary>
        /// Constructor having MusicBrainService that holds logic
        /// </summary>
        public MusicWikiController(IMusicBrainzService musicBrainzService)
        {
            _musicBrainzService = musicBrainzService;
        }

        /// <summary>
        /// Fetches the artist info by passing the mbId as routing element.
        /// Usage : GET api/musicwiki/[mbId]
        /// </summary>
        /// <remarks>
        /// Sample MbId of (Nirvana): 5b11f4ce-a62d-471e-81fc-a69a8278c7da
        /// </remarks>
        /// <param name="mbId">5b11f4ce-a62d-471e-81fc-a69a8278c7da</param>
        /// <returns>json format of Music Brainz info with album details</returns>
        [ProducesResponseType(typeof(List<MusicBrainz>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestException), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NoContentException), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundException), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        [HttpGet("{mbId}")]
        public async Task<IActionResult> Get(string mbId)
        {
            var musicBrainz = await _musicBrainzService.GetMusicBrainz(mbId);
            return Ok(musicBrainz);
        }
    }
}
