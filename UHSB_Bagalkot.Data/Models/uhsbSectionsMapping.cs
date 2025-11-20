using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbSectionsMapping
{
    public int SectionMapId { get; set; }

    public int SectionId { get; set; }

    public int CropId { get; set; }

    public virtual ICollection<UhsbItemDeail> UhsbItemDeails { get; set; } = new List<UhsbItemDeail>();
}
