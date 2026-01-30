using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHSB_Bagalkot.Service.ViewModels.CartOrder
{
    public class OrderMasterVM
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public byte OrderDataStatusType { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public int CreateBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
         
    }
    public class OrderItemVM
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int VarietyId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreateBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }

}
