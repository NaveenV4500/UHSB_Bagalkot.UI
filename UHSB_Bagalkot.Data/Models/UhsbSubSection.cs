using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbSubSection
{
    public int SubSectionId { get; set; }

    public int SectionId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual UhsbSection Section { get; set; } = null!;

    public virtual ICollection<UhsbItemDeail> UhsbItemDeails { get; set; } = new List<UhsbItemDeail>();
}
