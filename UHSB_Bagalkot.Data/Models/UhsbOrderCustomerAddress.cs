using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbOrderCustomerAddress
{
    public int OrderAddressId { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = null!;

    public string? District { get; set; }

    public string State { get; set; } = null!;

    public string Pincode { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual UhsbOrderMaster Order { get; set; } = null!;

    public virtual UserMaster User { get; set; } = null!;
}
