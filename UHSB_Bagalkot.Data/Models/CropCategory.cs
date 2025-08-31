using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class CropCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Crop> Crops { get; set; } = new List<Crop>();
}
