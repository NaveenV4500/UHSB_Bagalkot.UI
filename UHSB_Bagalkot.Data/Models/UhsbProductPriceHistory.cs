using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbProductPriceHistory
{
    public int PriceHistoryId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? Remarks { get; set; }

    public virtual UhsbProduct Product { get; set; } = null!;
}
