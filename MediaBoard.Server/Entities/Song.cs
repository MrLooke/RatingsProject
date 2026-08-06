using System.ComponentModel.DataAnnotations.Schema;

namespace MediaBoard.Server.Entities;

public partial class Song
{
    public int Id { get; set; }

    public int AlbumId { get; set; }

    public string Title { get; set; } = null!;

    [Column("track_number")]
    public int TrackNumber { get; set; }

    public string? Position { get; set; }

    public string? Duration { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual ICollection<SongRating> SongRatings { get; set; } = new List<SongRating>();
}
