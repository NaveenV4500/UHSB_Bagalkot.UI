using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(OrderMasterVM order);
        Task AddOrderItemsAsync(List<OrderItemVM> items);
        Task<OrderMasterVM> GetOrderByIdAsync(int orderId);
        Task<List<OrderItemVM>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<List<OrderMasterVM>> GetOrdersByUserIdAsync(int userId);
    }
}
