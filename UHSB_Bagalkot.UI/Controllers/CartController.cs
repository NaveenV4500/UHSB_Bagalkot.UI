using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.BiDi.Browser;
using UHSB_Bagalkot.Data.Models;
using UHSB_Bagalkot.Service.Interface;
using UHSB_Bagalkot.Service.ViewModels.CartOrder;

namespace UHSB_Bagalkot.UI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController] 
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICurrentUser _user;
         

        public CartController(ICartRepository cartRepository,ICurrentUser user)
        {
            _cartRepository = cartRepository;
            _user = user;
        }

        [HttpGet("cartcount")]
        public IActionResult GetCartCount()
        {
            if(_user == null || _user.UserId ==0)
            {
                return BadRequest();
            }

            var count = _cartRepository.GetCartCount(_user.UserId);
            return Ok(count);
        }

        [HttpGet("cartitems")]
        public IActionResult GetCartItems()
        {
            var data = _cartRepository.GetCartItems(_user.UserId);
            return Ok(data);
        }

        [HttpPost("addtocart")]
        public IActionResult AddToCart([FromBody] CartItemVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_user == null || _user.UserId == 0)
            {
                return BadRequest();
            }
            model.UserId = _user.UserId;
            var cartId = _cartRepository.GetOrCreateCartId(model.UserId);   

            _cartRepository.AddOrUpdateCartItem(
                model.UserId,
                cartId,
                model.ProductId,
                model.VarietyId,
                model.Quantity??0,
                model.Price ?? 0
            );

            return Ok("Item added to cart");
        }

        [HttpPut("updatequantity")]
        public IActionResult UpdateQuantity(int cartItemId = 0,int qty=0)
        {
            _cartRepository.UpdateQuantity(cartItemId, qty);
            return Ok("Quantity updated");
        }

        [HttpDelete("removeItemcart/{cartItemId}")]
        public IActionResult RemoveItem(int cartItemId)
        {
            _cartRepository.RemoveItem(cartItemId);
            return Ok("Item removed");
        }

        [HttpDelete("cartclear")]
        public IActionResult ClearCart()
        {
            if(_user == null || _user.UserId == 0)
            {
                return BadRequest();
            }
            _cartRepository.ClearCart(_user.UserId);
            return Ok("Cart cleared");
        }
    }
}
