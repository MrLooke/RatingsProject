using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public record ArtistSearchDTO(int? Id, string Name, long? RankScore)
    {
        public static ArtistSearchDTO FromEntity(SearchRanking artist)
        {
            return new ArtistSearchDTO(artist.ArtistId, artist.Name, artist.RankScore);
        }
    }
}
