using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbOrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int VarietyId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedDate { get; set; }

    public int CreateBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int ModifiedBy { get; set; }

    public byte OrderDataStatusType { get; set; }

    public virtual UhsbOrderMaster Order { get; set; } = null!;
}
