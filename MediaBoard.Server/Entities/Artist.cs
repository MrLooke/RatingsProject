using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? RealName { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ArtistMetric? ArtistMetric { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
