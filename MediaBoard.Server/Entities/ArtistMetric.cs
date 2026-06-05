using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class ArtistMetric
{
    public int ArtistId { get; set; }

    public int? TotalClicks { get; set; }

    public virtual Artist Artist { get; set; } = null!;
}
