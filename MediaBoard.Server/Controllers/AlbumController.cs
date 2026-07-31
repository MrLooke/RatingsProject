using MediaBoard.Server.Features.Album;
using Microsoft.AspNetCore.Mvc;

namespace MediaBoard.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet("{albumId:int}")]
        public async Task<ActionResult<AlbumInfoResult>> GetById(int albumId)
        {
            AlbumInfoResult? albumData = await _albumService.GetFullAlbumDataAsync(albumId);

            if (albumData is null) return NoContent();

            return Ok(albumData);
        }
    }
}
