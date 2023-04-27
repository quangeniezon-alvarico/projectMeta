using System;
using System.Collections.Generic;

namespace ProjectMetaAPI.Models;

public partial class Directory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;
}
