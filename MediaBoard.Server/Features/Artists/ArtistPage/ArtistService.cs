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

        public async Task<Artist> GetArtistAsync(int artistId)
        {
            Artist? artist = await _dbContext.Artists.AsNoTracking().Where(a => a.Id == artistId).FirstOrDefaultAsync();
            if (artist == null)
            {
                throw new KeyNotFoundException("Artist ID was not found.");
            }

            return artist;
        }
    }
}
