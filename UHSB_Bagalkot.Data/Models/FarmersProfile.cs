using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class FarmersProfile
{
    public int FarmerId { get; set; }

    public string Name { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string? Village { get; set; }

    public decimal? LandSize { get; set; }

    public DateTime? CreatedDate { get; set; }
}
