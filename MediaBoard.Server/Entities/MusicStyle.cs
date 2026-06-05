using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class MusicStyle
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
