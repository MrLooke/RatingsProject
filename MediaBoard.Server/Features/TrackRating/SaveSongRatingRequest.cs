using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Features.TrackRating
{
    public class SaveSongRatingRequest
    {
        [Required]
        public int SongId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
        public short Score { get; set; }

        public string? Review { get; set; }
    }
}
