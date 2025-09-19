using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbItemQnA
{
    public int QnAid { get; set; }

    public int ItemId { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public byte? Datastatus { get; set; }

    public int? UserId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual UhsbItemDeail Item { get; set; } = null!;
}
