using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbUnitMaster
{
    public int UnitId { get; set; }

    public string UnitNameEng { get; set; } = null!;

    public string UnitNameKnd { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<UhsbProductVariety> UhsbProductVarieties { get; set; } = new List<UhsbProductVariety>();
}
