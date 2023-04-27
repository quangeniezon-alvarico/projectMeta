using System;
using System.Collections.Generic;

namespace ProjectMetaAPI.Models;

public partial class Transaction
{
    public int TransId { get; set; }

    public string UserId { get; set; } = null!;

    public int ProductId { get; set; }

    public string TransQuan { get; set; } = null!;

    public string TransSize { get; set; } = null!;

    public decimal TransTotal { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
