using MediaBoard.Server.Features.AlbumRating;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] SaveRatingRequest request)
        {
            await _ratingService.SaveRating(request.UserId, request.AlbumId, request.Review, request.Score);
            return Ok();
        }
    }
}
