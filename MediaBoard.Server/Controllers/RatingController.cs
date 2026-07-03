using MediaBoard.Server.Exceptions;
using MediaBoard.Server.Features.AlbumRating;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MediaBoard.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SaveRatingRequest request)
        {
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Expected claim type id is null.");

            if (!int.TryParse(userIdString, out int userId))
                throw new UnauthorizedException("Expected claim type id is invalid.");

            await _ratingService.SaveRating(userId, request.AlbumId, request.Review, request.Score);
            return Ok();
        }
    }
}
