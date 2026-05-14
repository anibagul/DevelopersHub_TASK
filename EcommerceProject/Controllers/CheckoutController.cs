using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "CartItems");
            }

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "CartItems");
            }

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    TempData["Error"] = "One of the products no longer exists.";
                    return RedirectToAction("Index", "CartItems");
                }

                if (item.Product.Stock < item.Quantity)
                {
                    TempData["Error"] = $"Not enough stock available for {item.Product.Name}.";
                    return RedirectToAction("Index", "CartItems");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var totalAmount = cartItems.Sum(item => item.Quantity * item.Product!.Price);

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product!.Price
                    };

                    _context.OrderDetails.Add(orderDetail);

                    item.Product.Stock -= item.Quantity;
                }

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Your order has been placed successfully.";
                return RedirectToAction("Success", new { id = order.Id });
            }
            catch
            {
                await transaction.RollbackAsync();

                TempData["Error"] = "Something went wrong while placing your order.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Success(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}