using ECommerceStore.Data;
using ECommerceStore.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.isPaid)
                .SumAsync(o => o.TotalAmount);

            var ordersPerMonth = _context.Orders
                .Where(o => o.isPaid)
                .AsEnumerable() // 👈 switch from SQL to C# at this point
                .GroupBy(o => o.OrderDate.ToString("yyyy-MM"))
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Month, x => x.Count);

            var result = new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                OrdersPerMonth = ordersPerMonth
            };
            return Ok(result);

        }
    }
}
