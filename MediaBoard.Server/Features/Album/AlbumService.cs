using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.Album
{
    public class AlbumService : IAlbumService
    {

        private readonly AppDbContext _dbContext;

        public AlbumService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AlbumInfoResult?> GetFullAlbumDataAsync(int albumId)
        {
            var albumDetails = await _dbContext
                .Albums
                .AsNoTracking()
                .Where(a => a.Id == albumId)
                .Select(a => new AlbumInfoResult
                {
                    Id = a.Id,
                    Title = a.Title,
                    Year = a.Year,
                    Format = a.Format,
                    ImageUrl = a.ImageUrl,
                    Songs = a.Songs
                        .OrderBy(s => s.TrackNumber)
                        .Select(s => new AlbumSong
                        { 
                            Id = s.Id,
                            Title = s.Title,
                            TrackNumber = s.TrackNumber,
                            Position = s.Position,
                            Duration = s.Duration
                        }),
                    Artists = a.Artists.Select(art => new AlbumArtist { Id = art.Id, Name = art.Name })
                })
                .FirstOrDefaultAsync();

            return albumDetails;
        }
    }
}
