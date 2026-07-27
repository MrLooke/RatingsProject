using MediaBoard.Server.Features.Songs;
using Microsoft.AspNetCore.Mvc;

namespace MediaBoard.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet("{albumid:int}")]
        public async Task<ActionResult<IEnumerable<ReleaseSongResult>>> GetSongsByAldum(int albumId)
        {
            IEnumerable<ReleaseSongResult> songs = await _songService.GetSongsInAlbumAsync(albumId);
            return Ok(songs);
        }
    }
}
