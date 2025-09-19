using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbSection
{
    public int SectionId { get; set; }

    public int CropId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual UhsbCrop Crop { get; set; } = null!;

    public virtual ICollection<UhsbSubSection> UhsbSubSections { get; set; } = new List<UhsbSubSection>();
}
