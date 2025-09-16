using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class ItemContent
{
    public int ContentId { get; set; }

    public int ItemId { get; set; }

    public string? Title { get; set; }

    public string Article { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual UhsbItemDeail Item { get; set; } = null!;
}
