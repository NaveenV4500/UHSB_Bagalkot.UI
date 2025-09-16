using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbItemImage
{
    public int ImageId { get; set; }

    public int ItemId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Description { get; set; }

    public virtual UhsbItemDeail Item { get; set; } = null!;
}
