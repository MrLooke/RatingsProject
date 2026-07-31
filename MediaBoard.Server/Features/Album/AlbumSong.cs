namespace MediaBoard.Server.Features.Album
{
    public class AlbumSong
    {
        public required int Id { get; set; }

        public required string Title { get; set; }

        public required int TrackNumber { get; set; }

        public string? Position { get; set; }

        public string? Duration { get; set; }
    }
}
