namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public interface IArtistService
    {
        Task<ArtistPageDTO> GetArtistDetailsAsync(int artistId);
    }
}
