namespace MediaBoard.Server.Features.AlbumRating
{
    public interface IRatingService
    {
        Task SaveRating(int userId, int albumId, string? review, short rating);
    }
}
