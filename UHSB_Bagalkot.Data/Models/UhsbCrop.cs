using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbCrop
{
    public int CropId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual UhsbCategory Category { get; set; } = null!;

    public virtual ICollection<UhsbSection> UhsbSections { get; set; } = new List<UhsbSection>();
}
