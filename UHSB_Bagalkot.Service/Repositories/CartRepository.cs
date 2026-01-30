using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;

namespace UHSB_Bagalkot.Service.Repositories
{ 
     public class CartRepository : CommonConnection, ICartRepository
    {
        private readonly IMapper _mapper;
 
        public CartRepository(Uhsb2025uatContext context, IMapper mapper)
          : base(context)
        {
            _mapper = mapper;
        }
        public int GetCartCount(int userId)
        {
            return (from cm in _context.UhsbCartMasters
                    join ci in _context.UhsbCartItems on cm.CartId equals ci.CartId
                    where cm.UserId == userId 
                    select ci.ProductId).Count();
        }
         
        public int GetOrCreateCartId(int userId)
        {
            var cart = _context.UhsbCartMasters.FirstOrDefault(x => x.UserId == userId);
            if (cart != null) 
                return cart.CartId;

            cart = new UhsbCartMaster { UserId = userId };
            _context.UhsbCartMasters.Add(cart);
            _context.SaveChanges();
            return cart.CartId;
        }

        public void AddOrUpdateCartItem(int userId,int cartId, int productId, int varietyId, int qty, decimal price)
        {
            var item = _context.UhsbCartItems.FirstOrDefault(x =>
                x.CartId == cartId &&
                x.ProductId == productId &&
                x.VarietyId == varietyId);

            if (item != null)
            {
                item.Quantity += qty;
                item.ModifiedDate = DateTime.Now;
                item.ModifiedBy = userId;
            }
            else
            {
                _context.UhsbCartItems.Add(new UhsbCartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    VarietyId = varietyId,
                    Quantity = qty,
                    Price = price,
                    CreatedBy = userId,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now
                });
            }
            _context.SaveChanges();
        }

        public List<CartItemVM> GetCartItems(int userId)
        {
            if (userId <= 0)
                return new List<CartItemVM>();

            var data =
                (from cm in _context.UhsbCartMasters
                 join ci in _context.UhsbCartItems on cm.CartId equals ci.CartId
                 join pv in _context.UhsbProductVarieties on ci.VarietyId equals pv.VarietiesId
                 join p in _context.UhsbProducts on pv.ProductId equals p.ProductId
                 join c in _context.UhsbSeedPlantingCenterMasters on pv.CenterId equals c.CenterId
                 join d in _context.UhsbDistricts on c.DistrictId equals d.DistrictId
                 where cm.UserId == userId
                 select new CartItemVM
                 {
                     CartItemId = ci.CartItemId,

                     ProductId = p.ProductId,
                     ProductNameEng = p.ProductNameKnd,
                     ProductNameKnd = p.ProductNameKnd,

                     VarietyId = pv.VarietiesId,
                     VarietyNameEng = pv.VarietyNameEng,
                     VarietyNameKnd = pv.VarietyNameKnd,

                     CenterId = c.CenterId,
                     CenterName = c.CenternameEng, 

                     DistrictName = d.DistrictName,

                     Quantity = ci.Quantity,
                     Price = ci.Price,

                     AvailableStock = pv.StockQty,
                     IsStockAvailable = pv.StockQty >= ci.Quantity
                 }).ToList();

            return data;
        }



        public void UpdateQuantity(int cartItemId, int qty)
        {
            var item = _context.UhsbCartItems.FirstOrDefault(x => x.CartItemId == cartItemId);
            if (item == null) return;

            item.Quantity = qty;
            item.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
        }

        public bool RemoveItem(int cartItemId)
        {
            try
            {
                var item = _context.UhsbCartItems.FirstOrDefault(x => x.CartItemId == cartItemId);
                if (item == null) return false;

                _context.UhsbCartItems.Remove(item);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
          
        }

        public bool ClearCart(int userId)
        {
            try
            {
                var cart = _context.UhsbCartMasters.FirstOrDefault(x => x.UserId == userId);
                if (cart == null) return false;

                var items = _context.UhsbCartItems.Where(x => x.CartId == cart.CartId);
                _context.UhsbCartItems.RemoveRange(items);
                _context.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }
            
        }
    }
}
