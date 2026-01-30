using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbProduct
{
    public int ProductId { get; set; }

    public int CenterId { get; set; }

    public int HeadId { get; set; }

    public string ProductNameEng { get; set; } = null!;

    public string ProductNameKnd { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? Filepath { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int DistrictId { get; set; }

    public virtual UhsbSeedPlantingCenterMaster Center { get; set; } = null!;

    public virtual UhsbDistrict District { get; set; } = null!;

    public virtual UhsbRecordHeadMaster Head { get; set; } = null!;

    public virtual ICollection<UhsbProductVariety> UhsbProductVarieties { get; set; } = new List<UhsbProductVariety>();
}
