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

        public async Task<ArtistPageDTO?> GetArtistDetailsAsync(int artistId, int? userId = null)
        {
            var artist = await _dbContext
                .Artists
                .AsNoTracking()
                .Where(a => a.Id == artistId)
                .Select(a => new { a.Id, a.Name, a.Description })
                .FirstOrDefaultAsync();

            if (artist is null) return null;

            var albums = await GetAlbumsForArtistAsync(artistId, userId);
            var topSongs = await GetTopSongsForArtistAsync(artistId, userId);

            return new ArtistPageDTO(artist.Id, artist.Name, artist.Description, albums, topSongs);
        }

        public async Task<List<AlbumDTO>> GetAlbumsForArtistAsync(int artistId, int? userId)
        {
            var albums = await _dbContext
                .Albums
                .AsNoTracking()
                .Where(a => a.Artists.Any(art => art.Id == artistId))
                .OrderBy(a => a.Year == null)
                .ThenByDescending(a => a.Year)
                .Select(a => new AlbumDTO(
                    a.Id,
                    a.Title,
                    a.Year,
                    a.Format,
                    a.Ratings.Count > 0 ? a.Ratings.Average(r => r.Score) : null,
                    userId.HasValue
                        ? a.Ratings.Where(r => r.UserId == userId).Select(r => (short?)r.Score).FirstOrDefault()
                        : null
                ))
                .ToListAsync();

            return albums;
        }

        public async Task<List<TopSongDTO>> GetTopSongsForArtistAsync(int artistId, int? userId)
        {
            var songs = await _dbContext
                .Songs
                .AsNoTracking()
                .Where(s => s.Album.Artists.Any(art => art.Id == artistId))
                .OrderByDescending(s => s.SongRatings.Average(r => r.Score))
                .Take(10)
                .Select(s => new TopSongDTO(
                    s.Id,
                    s.Title,
                    s.Album.Id,
                    s.Album.Title,
                    s.SongRatings.Average(r => r.Score)
                ))
                .ToListAsync();

            return songs;
        }
    }
}
