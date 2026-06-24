using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaBoard.Server.Entities;

public partial class Rating
{
    public int UserId { get; set; }

    public int MediaId { get; set; }

    public string? Review { get; set; }

    [Column("rating")] public short Score { get; set; }

    public virtual AppUser? User { get; set; }
    public virtual Album? Album { get; set; }
}
