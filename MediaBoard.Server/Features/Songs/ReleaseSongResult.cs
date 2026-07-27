using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Features.Songs
{
    public class ReleaseSongResult
    {

        public required int Id { get; set; }

        public required string Title { get; set; }

        public string? Duration { get; set; }
    }
}
