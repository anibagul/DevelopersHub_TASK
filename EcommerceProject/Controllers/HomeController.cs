using EcommerceProject.Data;
using EcommerceProject.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync(),

                RecommendedProducts = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.Id)
                    .Take(10)
                    .ToListAsync(),

                DealsProducts = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Price)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}