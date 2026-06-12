using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Artists.ArtistPage
{
    public interface IArtistService
    {
        Task<Artist> GetArtistAsync(int artistId);
    }
}
