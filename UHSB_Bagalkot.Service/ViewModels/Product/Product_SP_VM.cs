using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.Product
{
    public class Product_SP_VM
    {
        public int ProductId { get; set; }

        public int CenterId { get; set; }
        public string? DistrictName { get; set; }

        public string? Centername_eng { get; set; }

        public string? RecordHead_eng { get; set; }

        public string? ProductName_eng { get; set; }

        public string? ProductName_knd { get; set; }

        public string? Remarks { get; set; }

        public string? Filepath { get; set; }

        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

    public class ProductVariety_SP_VM
    {
        public int VarietiesId { get; set; }
        public int ProductId { get; set; }

        public string ProductName_eng { get; set; }
        public string ProductName_knd { get; set; }

        public int CenterId { get; set; }
        public string Centername_eng { get; set; }

        public string DistrictName { get; set; }

        public string VarietyName_eng { get; set; }
        public string VarietyName_knd { get; set; }

        public string Stock_Keeping_Unit { get; set; }
        public string Barcode { get; set; }

        public int? UnitId { get; set; }
        public string UnitName_eng { get; set; }

        public decimal? Quantity { get; set; }
        public decimal MRPPrice { get; set; }
        public decimal SellingPrice { get; set; }

        public int StockQty { get; set; }
        public int? MinStockQty { get; set; }

        public bool IsActive { get; set; }

        public string filepath { get; set; }
        public string Remarks { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}
