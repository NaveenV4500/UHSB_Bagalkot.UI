using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbItemDeail
{
    public int ItemId { get; set; }

    public int SubSectionId { get; set; }

    public string Name { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public virtual ICollection<ItemContent> ItemContents { get; set; } = new List<ItemContent>();

    public virtual UhsbSubSection SubSection { get; set; } = null!;

    public virtual ICollection<UhsbItemImage> UhsbItemImages { get; set; } = new List<UhsbItemImage>();

    public virtual ICollection<UhsbItemQnA> UhsbItemQnAs { get; set; } = new List<UhsbItemQnA>();
}
