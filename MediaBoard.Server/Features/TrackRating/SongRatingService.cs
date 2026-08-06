using MediaBoard.Server.Entities;
using MediaBoard.Server.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.TrackRating
{
    public class SongRatingService : ISongRatingService
    {
        private readonly AppDbContext _dbContext;
        public SongRatingService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SaveSongRating(int userId, int songId, string? review, short rating)
        {
            var song = await _dbContext.Songs
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == songId)
                ?? throw new NotFoundException($"Song {songId} not found.");

            var existing = await _dbContext.SongRatings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.SongId == songId);

            if (existing is null)
            {
                var newRating = new SongRating
                {
                    UserId = userId,
                    SongId = songId,
                    AlbumId = song.AlbumId,
                    Score = rating,
                    Review = review
                };
                _dbContext.SongRatings.Add(newRating);
            }
            else
            {
                existing.Score = rating;
                existing.Review = review;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
