using ECommerceStore.Data;
using ECommerceStore.DTOs;
using ECommerceStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == userId && c.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }

            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                _context.CartItems.Add(cartItem);

            }

            await _context.SaveChangesAsync();
            return Ok("Item added to cart.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var cart = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => new
                {
                    c.Id,
                    c.ProductId,
                    c.Product.Name,
                    c.Product.Price,
                    c.Product.ImageUrl,
                    c.Quantity,
                    Total = c.Quantity * c.Product.Price
                })
                .ToListAsync();

            return Ok(cart);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == dto.CartItemId && c.UserId == userId);

            if (item == null)
                return NotFound("Cart item not found.");

            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
            }

            await _context.SaveChangesAsync();
            return Ok("Cart item updated.");
        }

        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (item == null)
                return NotFound("Cart item not found.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Item removed from cart.");
        }

    }
}

