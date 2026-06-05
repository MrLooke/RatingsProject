using System;
using System.Collections.Generic;

namespace MediaBoard.Server.Entities;

public partial class Rating
{
    public int? UserId { get; set; }

    public string MediaId { get; set; } = null!;

    public string? Review { get; set; }

    public short? Rating1 { get; set; }

    public virtual AppUser? User { get; set; }
}
