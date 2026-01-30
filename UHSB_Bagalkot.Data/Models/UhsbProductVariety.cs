using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbProductVariety
{
    public int VarietiesId { get; set; }

    public int ProductId { get; set; }

    public int CenterId { get; set; }

    public string? VarietyNameEng { get; set; }

    public string? VarietyNameKnd { get; set; }

    public string? StockKeepingUnit { get; set; }

    public string? Barcode { get; set; }

    public int? UnitId { get; set; }

    public decimal? Quantity { get; set; }

    public decimal Mrpprice { get; set; }

    public decimal SellingPrice { get; set; }

    public int StockQty { get; set; }

    public int? MinStockQty { get; set; }

    public bool IsActive { get; set; }

    public string? Filepath { get; set; }

    public string? Remarks { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual UhsbSeedPlantingCenterMaster Center { get; set; } = null!;

    public virtual UhsbProduct Product { get; set; } = null!;

    public virtual UhsbUnitMaster? Unit { get; set; }
}
