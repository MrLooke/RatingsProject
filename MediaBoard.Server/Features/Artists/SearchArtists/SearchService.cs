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

            var artistObjects = await query
                .OrderByDescending(a => a.RankScore)
                .ThenByDescending(a => a.ArtistId)
                .Take(limit)
                .ToListAsync();

            IEnumerable<ArtistSearchDTO> artistDtos = artistObjects.Where(a => a.ArtistId.HasValue).Select(ArtistSearchDTO.FromEntity);
            return artistDtos;
        }
    }
}
