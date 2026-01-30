using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;

namespace UHSB_Bagalkot.Service.ViewModels.CartOrder
{
    public class CartItemVM
    { 
            public int? CartItemId { get; set; }
            public int UserId { get; set; }

            public int ProductId { get; set; }
            public string? ProductNameEng { get; set; }
            public string? ProductNameKnd { get; set; }

            public int VarietyId { get; set; }
            public string? VarietyNameEng { get; set; }
            public string? VarietyNameKnd { get; set; }

            public int CenterId { get; set; }
            public string? CenterName { get; set; }

            public string? DistrictName { get; set; }

            public int? Quantity { get; set; }
            public decimal? Price { get; set; }

            public int? AvailableStock { get; set; }
            public bool? IsStockAvailable { get; set; }
        }

    } 