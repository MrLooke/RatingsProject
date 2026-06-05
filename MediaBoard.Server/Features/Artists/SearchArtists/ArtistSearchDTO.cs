using MediaBoard.Server.Entities;

namespace MediaBoard.Server.Features.Artists.SearchArtists
{
    public record ArtistSearchDTO(int Id, string Name, string Description = "")
    {
        public static ArtistSearchDTO FromEntity(Artist artist)
        {
            return new ArtistSearchDTO(artist.Id, artist.Name, artist.Description ?? "");
        }
    }
}
