namespace MediaBoard.Server.Features.TrackRating
{
    public interface ISongRatingService
    {
        Task SaveSongRating(int userId, int songId, string? review, short rating);
    }
}
