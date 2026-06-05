using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class Album
{
    public int Id { get; set; }

    public int? MainId { get; set; }

    public string Title { get; set; } = null!;

    public int? Year { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Artist> Artists { get; set; } = new List<Artist>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<MusicStyle> Styles { get; set; } = new List<MusicStyle>();
}
