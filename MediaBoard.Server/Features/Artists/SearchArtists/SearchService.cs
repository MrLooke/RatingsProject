using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _dbContext;
        public SearchService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<ArtistSearchDTO>> GetArtistBySearchAsync(string searchString, int limit = 15, int? lastId = null, int? lastRankScore = null)
        {
            var query = _dbContext.SearchRankings
                .AsNoTracking()
                .Where(a => EF.Functions.ILike(a.Name, $"{searchString}%"));

            if (lastId.HasValue && lastRankScore.HasValue)
            {
                query = query.Where(x => x.RankScore < lastRankScore || (x.RankScore == lastRankScore && x.ArtistId < lastId));
            }

            var artistDtos = await query
                .Where(a => a.ArtistId != null)
                .OrderByDescending(a => a.RankScore)
                .ThenByDescending(a => a.ArtistId)
                .Take(limit)
                .Select(a => new ArtistSearchDTO(a.ArtistId!.Value, a.Name ?? "", a.RankScore))
                .ToListAsync();

            return artistDtos;
        }
    }
}
