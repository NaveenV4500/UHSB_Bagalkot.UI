using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;

namespace UHSB_Bagalkot.Service.Interface
{
    public interface  ICartRepository
    {
        int GetCartCount(int userId);
        int GetOrCreateCartId(int userId);
        void AddOrUpdateCartItem(int userId,int cartId, int productId, int varietyId, int qty, decimal price);
        List<CartItemVM> GetCartItems(int userId);
        void UpdateQuantity(int cartItemId, int qty);
        bool RemoveItem(int cartItemId);
        bool ClearCart(int userId);
    }
}
