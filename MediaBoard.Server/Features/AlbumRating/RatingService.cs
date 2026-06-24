using MediaBoard.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBoard.Server.Features.AlbumRating
{
    public class RatingService
    {
        private readonly AppDbContext _dbContext;
        public RatingService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SaveRating(int userId, string albumId, string review, short rating)
        {
            ValidateRating(userId, albumId, review, rating);

            var existing = await _dbContext.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MediaId == albumId);

            if (existing is null)
            {
                var newReview = new Rating
                {
                    UserId = userId,
                    MediaId = albumId,
                    Score = rating,
                    Review = review
                };
                _dbContext.Ratings.Add(newReview);
            }
            else
            {
                existing.Score = rating;
                existing.Review = review;
            }

            await _dbContext.SaveChangesAsync();
        }

        private void ValidateRating(int userId, string albumId, string review, short rating)
        {
            if (rating < 1 || rating > 10)
            {
                throw new ArgumentOutOfRangeException("Rating must be inclusively between 1 and 10");
            }

            if(string.IsNullOrEmpty(albumId))
            {
                throw new ArgumentNullException("Album ID cannot be null or empty.");
            }
        }
    }
}
