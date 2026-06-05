using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _dbContext;
        public SearchService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        }

        public async Task<Artist> GetArtistAsync(int artistId)
        {
            Artist? artist = await _dbContext.Artists.AsNoTracking().Where(a => a.Id == artistId).FirstOrDefaultAsync();
            if(artist == null)
            {
                throw new KeyNotFoundException("Artist ID was not found.");
            }

            return artist;
        }

        public async Task<IEnumerable<ArtistSearchDTO>> GetArtistBySearchAsync(string searchString, int limit = 15, int lastId = -1, int lastRankScore = -1)
        {
            var query = _dbContext.SearchRankings
                .AsNoTracking()
                .Where(a => EF.Functions.ILike(a.Name, $"{searchString}%"));


            if (lastId != -1 && lastRankScore != -1)
            {
                query = query.Where(x => x.RankScore < lastRankScore || (x.RankScore == lastRankScore && x.ArtistId > lastId));
            }

            var artistObjects = await query
                .OrderByDescending(a => a.RankScore)
                .ThenBy(a => a.ArtistId)
                .Take(limit)
                .ToListAsync();

            IEnumerable<ArtistSearchDTO> artistDtos = artistObjects.Select(a => new ArtistSearchDTO(a.ArtistId!.Value, a.Name ?? ""));
            return artistDtos;
        }
    }
}
