namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public record ArtistPageDTO(int Id, string Name, string? Description, IList<AlbumDTO> albums)
    {
    }
}
