using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbSeedPlantingCenterMaster
{
    public int CenterId { get; set; }

    public int? DistrictId { get; set; }

    public string? CenternameEng { get; set; }

    public string? CenternameKnd { get; set; }

    public virtual UhsbDistrict? District { get; set; }

    public virtual ICollection<UhsbAvailabilityToolsDetail> UhsbAvailabilityToolsDetails { get; set; } = new List<UhsbAvailabilityToolsDetail>();
}
