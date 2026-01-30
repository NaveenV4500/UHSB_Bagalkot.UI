using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbOrderMaster
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public byte OrderDataStatusType { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreateBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int ModifiedBy { get; set; }

    public virtual ICollection<UhsbOrderCustomerAddress> UhsbOrderCustomerAddresses { get; set; } = new List<UhsbOrderCustomerAddress>();

    public virtual ICollection<UhsbOrderItem> UhsbOrderItems { get; set; } = new List<UhsbOrderItem>();

    public virtual UserMaster User { get; set; } = null!;
}
