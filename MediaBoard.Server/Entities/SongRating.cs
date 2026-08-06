using System.ComponentModel.DataAnnotations.Schema;

namespace MediaBoard.Server.Entities
{
    public class SongRating
    {
        public int UserId { get; set; }

        public int SongId { get; set; }

        public int AlbumId { get; set; }

        [Column("rating")] public short Score { get; set; }

        public string? Review { get; set; }

        public virtual AppUser User { get; set; } = null!;

        public virtual Song Song { get; set; } = null!;

        public virtual Album? Album { get; set; }
    }
}
