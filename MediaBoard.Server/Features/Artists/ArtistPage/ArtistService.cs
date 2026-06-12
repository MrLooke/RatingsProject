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

        public async Task<ArtistPageDTO> GetArtistDetailsAsync(int artistId)
        {
            var artistDetails = await _dbContext
                .Artists
                .AsNoTracking()
                .Where(a => a.Id == artistId)
                .Select(a => new ArtistPageDTO(a.Id, a.Name,
                    a.Albums
                        .OrderByDescending(al => al.Year)
                        .Select(al => new AlbumDTO(al.Id, al.Title, al.Year))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            return artistDetails;
        }
    }
}
