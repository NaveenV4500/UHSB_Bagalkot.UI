using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbItemImage
{
    public int ImageId { get; set; }

    public int ItemId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual UhsbItemDeail Item { get; set; } = null!;
}
