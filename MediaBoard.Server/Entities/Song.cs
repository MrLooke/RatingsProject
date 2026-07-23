using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class Song
{
    public int Id { get; set; }

    public int AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public string? Position { get; set; }

    public string? Duration { get; set; }

    public virtual Album Album { get; set; } = null!;
}
