using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbAvailabilityToolsDetail
{
    public int Identifier { get; set; }

    public int CenterId { get; set; }

    public int HeadId { get; set; }

    public string? AvailToolnameEng { get; set; }

    public string? AvailToolnameKnd { get; set; }

    public int? Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public DateOnly? AvailabilityDate { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual UhsbSeedPlantingCenterMaster Center { get; set; } = null!;

    public virtual UhsbRecordHeadMaster Head { get; set; } = null!;
}
