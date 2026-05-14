using EcommerceProject.Data;
using EcommerceProject.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalProducts = await _context.Products.CountAsync(p => p.IsActive);
            ViewBag.TotalOrders = await _context.Orders.CountAsync();
            ViewBag.TotalCartItems = await _context.CartItems.CountAsync();
            ViewBag.TotalOrderDetails = await _context.OrderDetails.CountAsync();
            ViewBag.TotalSales = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.ProcessingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Processing);
            ViewBag.ShippedOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Shipped);
            ViewBag.DeliveredOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Delivered);
            ViewBag.CancelledOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled);

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            return View(recentOrders);
        }
    }
}
