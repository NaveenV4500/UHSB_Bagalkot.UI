using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbItemDeail
{
    public int ItemId { get; set; }

    public int SubSectionId { get; set; }

    public string? Name { get; set; }

    public string? ImageUrl { get; set; }

    public int SectionMapId { get; set; }

    public int CropId { get; set; }

    public int CategoryId { get; set; }

    public int SectionId { get; set; }

    public virtual ICollection<ItemContent> ItemContents { get; set; } = new List<ItemContent>();

    public virtual UhsbSectionsMapping SectionMap { get; set; } = null!;

    public virtual ICollection<UhsbItemImage> UhsbItemImages { get; set; } = new List<UhsbItemImage>();

    public virtual ICollection<UhsbItemQnA> UhsbItemQnAs { get; set; } = new List<UhsbItemQnA>();
}
