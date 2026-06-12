namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public record ArtistPageDTO(int Id, string Name, IList<AlbumDTO> albums)
    {
    }
}
