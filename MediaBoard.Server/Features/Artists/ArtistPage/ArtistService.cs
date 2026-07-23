using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public class ArtistService : IArtistService
    {
        private readonly AppDbContext _dbContext;
        public ArtistService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        }

        public async Task<ArtistPageDTO> GetArtistDetailsAsync(int artistId, int? userId = null)
        {
            var artistDetails = await _dbContext
                .Artists
                .AsNoTracking()
                .Where(a => a.Id == artistId)
                .Select(a => new ArtistPageDTO(a.Id, a.Name, a.Description,
                    a.Albums
                        .OrderBy(a => a.Year == null)
                        .ThenByDescending(al => al.Year)
                        .Select(al => new AlbumDTO
                        (
                            al.Id, 
                            al.Title, 
                            al.Year, 
                            al.Format, 
                            al.Ratings.Any() ? al.Ratings.Average(r => r.Score) : null,
                            userId.HasValue 
                                ? al.Ratings.Where(r => r.UserId == userId).Select(r => (short?)r.Score).FirstOrDefault()
                                : null
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            return artistDetails;
        }
    }
}
