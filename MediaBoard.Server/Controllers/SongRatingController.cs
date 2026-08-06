using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Features.TrackRating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediaBoard.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SongRatingController : ControllerBase
    {
        private readonly ISongRatingService _songRatingService;

        public SongRatingController(ISongRatingService songRatingService)
        {
            _songRatingService = songRatingService;
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SaveSongRatingRequest request)
        {
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Expected claim type id is null.");

            if (!int.TryParse(userIdString, out int userId))
                throw new UnauthorizedException("Expected claim type id is invalid.");

            await _songRatingService.SaveSongRating(userId, request.SongId, request.Review, request.Score);
            return Ok();
        }
    }
}
