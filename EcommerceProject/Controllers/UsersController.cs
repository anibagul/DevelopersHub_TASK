using EcommerceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            return View(users);
        }

        // GET: Users/Details/user-id
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FullName,Address,Email,UserName,PhoneNumber")] ApplicationUser user,
            string password)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError(nameof(ApplicationUser.Email), "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = user.Email;
            }

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(user);
        }

        // GET: Users/Edit/user-id
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/user-id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("Id,FullName,Address,Email,UserName,PhoneNumber")] ApplicationUser formUser)
        {
            if (id != formUser.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(formUser.Email))
            {
                ModelState.AddModelError(nameof(ApplicationUser.Email), "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(formUser.UserName))
            {
                formUser.UserName = formUser.Email;
            }

            if (ModelState.IsValid)
            {
                user.FullName = formUser.FullName;
                user.Address = formUser.Address;
                user.Email = formUser.Email;
                user.UserName = formUser.UserName;
                user.PhoneNumber = formUser.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(formUser);
        }

        // GET: Users/Delete/user-id
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/user-id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("Delete", user);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}