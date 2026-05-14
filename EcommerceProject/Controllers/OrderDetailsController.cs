using EcommerceProject.Data;
using EcommerceProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .OrderByDescending(od => od.OrderId)
                .ToListAsync();

            return View(orderDetails);
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // GET: OrderDetails/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View();
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,OrderId,ProductId,Quantity,Price")] OrderDetail orderDetail)
        {
            ModelState.Remove(nameof(OrderDetail.Order));
            ModelState.Remove(nameof(OrderDetail.Product));

            if (orderDetail.Quantity <= 0)
            {
                ModelState.AddModelError(nameof(OrderDetail.Quantity), "Quantity must be greater than zero.");
            }

            if (orderDetail.Price < 0)
            {
                ModelState.AddModelError(nameof(OrderDetail.Price), "Price cannot be negative.");
            }

            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(orderDetail.OrderId, orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            await PopulateDropDownsAsync(orderDetail.OrderId, orderDetail.ProductId);
            return View(orderDetail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,OrderId,ProductId,Quantity,Price")] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(OrderDetail.Order));
            ModelState.Remove(nameof(OrderDetail.Product));

            if (orderDetail.Quantity <= 0)
            {
                ModelState.AddModelError(nameof(OrderDetail.Quantity), "Quantity must be greater than zero.");
            }

            if (orderDetail.Price < 0)
            {
                ModelState.AddModelError(nameof(OrderDetail.Price), "Price cannot be negative.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.OrderDetails.Update(orderDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(orderDetail.OrderId, orderDetail.ProductId);
            return View(orderDetail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(od => od.Id == id);
        }

        private async Task PopulateDropDownsAsync(
            object? selectedOrderId = null,
            object? selectedProductId = null)
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewData["OrderId"] = new SelectList(orders, "Id", "Id", selectedOrderId);

            var products = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();

            ViewData["ProductId"] = new SelectList(products, "Id", "Name", selectedProductId);
        }
    }
}