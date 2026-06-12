using MediaBoard.Server.Features.Artists.SearchArtists;
using Microsoft.AspNetCore.Mvc;

namespace MediaBoard.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ArtistController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public ArtistController(ISearchService service)
        {
            _searchService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtistSearchDTO>>> Get([FromQuery] string query, [FromQuery] int limit = 15, [FromQuery] int? lastId = null, [FromQuery] int? lastRankScore = null)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Seach query cannot be empty.");

            IEnumerable<ArtistSearchDTO> artistDtos = await _searchService.GetArtistBySearchAsync(query, limit, lastId, lastRankScore);
            return Ok(artistDtos);
        }
    }
}
