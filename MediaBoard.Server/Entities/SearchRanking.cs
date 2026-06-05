using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class SearchRanking
{
    public int? ArtistId { get; set; }

    public string? Name { get; set; }

    public long? AlbumCount { get; set; }

    public int? TotalClicks { get; set; }

    public long? RankScore { get; set; }
}
