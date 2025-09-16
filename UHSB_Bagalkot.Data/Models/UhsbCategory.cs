using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public virtual ICollection<UhsbCrop> UhsbCrops { get; set; } = new List<UhsbCrop>();
}
