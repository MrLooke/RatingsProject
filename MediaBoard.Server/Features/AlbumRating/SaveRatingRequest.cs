using System.ComponentModel.DataAnnotations;

namespace MediaBoard.Server.Features.AlbumRating
{
    public class SaveRatingRequest
    {
        [Required]
        public int AlbumId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Score must be between 1 and 10.")]
        public short Score { get; set; }

        public string? Review { get; set; }
    }
}
