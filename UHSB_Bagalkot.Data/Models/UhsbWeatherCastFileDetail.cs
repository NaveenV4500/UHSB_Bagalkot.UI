using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbWeatherCastFileDetail
{
    public int WeatherFileId { get; set; }

    public int? UserId { get; set; }

    public DateOnly? WeekStartDate { get; set; }

    public DateOnly? WeekEndDate { get; set; }

    public string? FilePath { get; set; }

    public string? Description { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int? DistrictId { get; set; }

    public virtual UhsbDistrict? District { get; set; }

    public virtual UserMaster? User { get; set; }
}
