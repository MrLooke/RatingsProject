using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public interface ISearchService
    {
        Task<Artist> GetArtistAsync(int artistId);
        Task<IEnumerable<ArtistSearchDTO>> GetArtistBySearchAsync(string searchString, int limit = 15, int lastId = -1, int lastRankScore = -1);
    }
}
