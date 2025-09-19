using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbDistrict
{
    public int DistrictId { get; set; }

    public string DistrictName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<UhsbWeatherCastFileDetail> UhsbWeatherCastFileDetails { get; set; } = new List<UhsbWeatherCastFileDetail>();
}
