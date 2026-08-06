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

        public async Task<AlbumInfoResult?> GetFullAlbumDataAsync(int albumId, int? userId = null)
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
                            Duration = s.Duration,
                            AverageRating = s.SongRatings.Any() ? s.SongRatings.Average(r => (double?)r.Score) : null,
                            UserRating = userId.HasValue
                                ? s.SongRatings.Where(r => r.UserId == userId).Select(r => (short?)r.Score).FirstOrDefault()
                                : null
                        }),
                    Artists = a.Artists.Select(art => new AlbumArtist { Id = art.Id, Name = art.Name })
                })
                .FirstOrDefaultAsync();

            return albumDetails;
        }
    }
}
