
namespace MediaBoard.Server.Features.Album
{
    public class AlbumArtist
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
    }


    public class AlbumInfoResult
    {
        public required int Id { get; set; }

        public required string Title { get; set; }

        public required IEnumerable<AlbumArtist> Artists { get; set; }

        public IEnumerable<AlbumSong>? Songs { get; set; }

        public int? Year { get; set; }

        public string? ImageUrl { get; set; }

        public string? Format { get; set; }
    }
}
