namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public record TopSongDTO(int Id, string Title, int AlbumId, string AlbumTitle, double? AverageRating)
    {
    }
}
