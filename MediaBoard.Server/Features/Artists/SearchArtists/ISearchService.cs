using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public interface ISearchService
    {
        Task<IEnumerable<ArtistSearchDTO>> GetArtistBySearchAsync(string searchString, int limit = 15, int? lastId = null, int? lastRankScore = null);
    }
}
