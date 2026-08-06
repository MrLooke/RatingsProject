using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Features.Album;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                string? idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(idString, out int parsedId))
                    throw new UnauthorizedException("Expected claim type id is invalid.");

                userId = parsedId;
            }

            AlbumInfoResult? albumData = await _albumService.GetFullAlbumDataAsync(albumId, userId);

            if (albumData is null) return NoContent();

            return Ok(albumData);
        }
    }
}
