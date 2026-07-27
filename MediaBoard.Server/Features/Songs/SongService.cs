using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.Songs
{
    public class SongService : ISongService
    {
        private readonly AppDbContext _dbContext;

        public SongService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<ReleaseSongResult>> GetSongsInAlbumAsync(int albumId)
        {
            var albumSongs = await _dbContext
                .Songs
                .AsNoTracking()
                .Where(s => s.AlbumId == albumId)
                .Select(s => new ReleaseSongResult { Title = s.Title, Duration = s.Duration })
                .ToListAsync();

            return albumSongs;
        }
    }
}
