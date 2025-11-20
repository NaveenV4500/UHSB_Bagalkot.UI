using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbRecordHeadMaster
{
    public int HeadId { get; set; }

    public string? RecordHeadEng { get; set; }

    public string? RecordHeadKnd { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<UhsbAvailabilityToolsDetail> UhsbAvailabilityToolsDetails { get; set; } = new List<UhsbAvailabilityToolsDetail>();
}
