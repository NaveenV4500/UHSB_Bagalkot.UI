using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UHSB_Bagalkot.Data.Models;
using static UHSB_Bagalkot.Service.Common.CommonEnum;

namespace UHSB_Bagalkot.Service.ViewModels.Product
{
    public class ProductsVM
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
     

        public IFormFile? ImageFile { get; set; }
        public  List<ProductVarietyVM> ProductVarietyItems { get; set; }
        public IEnumerable<SelectListItem> DistrictType { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Centerstype { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Centervariatestype { get; set; } = new List<SelectListItem>();

    }


    public class ProductVarietyVM
    {
        public int VarietiesId { get; set; }
        public int ProductId { get; set; }
        public int CenterId { get; set; }

        [Required(ErrorMessage = "Variety Name (English) is required")]
        public string? VarietyNameEng { get; set; }

        [Required(ErrorMessage = "Variety Name (Kannada) is required")]
        public string? VarietyNameKnd { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        public int? UnitId { get; set; }

        //[Required(ErrorMessage = "MRP is required")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "MRP must be greater than 0")]
        public decimal Mrpprice { get; set; } = 1;

        [Required(ErrorMessage = "Selling Price is required")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Selling Price must be greater than 0")]
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "Stock Qty is required")]
        
        public int StockQty { get; set; }

        //[Required(ErrorMessage = "Min Stock Qty is required")]
        //[Range(0, int.MaxValue, ErrorMessage = "Min Stock Qty cannot be negative")]
        public int? MinStockQty { get; set; } = 1;

        public bool IsDeleted { get; set; }
         
        public string? StockKeepingUnit { get; set; }

        public string? Barcode { get; set; }
         
        public decimal? Quantity { get; set; }

        public bool IsActive { get; set; }

        public string? Filepath { get; set; }

        public string? Remarks { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string? UnitName { get; set; }

        public UnitType? UnitEnum
        {
            get
            {
                if (!UnitId.HasValue)
                    return null;

                return Enum.IsDefined(typeof(UnitType), UnitId.Value)
                    ? (UnitType)UnitId.Value
                    : null;
            }
            set
            {
                if (value.HasValue)
                {
                    UnitId = (int)value.Value;
                    UnitName = value.Value.ToString();
                }
                else
                {
                    UnitId = null;
                    UnitName = null;
                }
            }
        }

        public IFormFile? ImageFile { get; set; } 
        public IEnumerable<SelectListItem> ProductList { get; set; } = new List<SelectListItem>(); 

    }
}
