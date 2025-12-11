using Microsoft.AspNetCore.Mvc;
using PRM.Models;
using PRM.Services;

namespace PRM.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthService authService, ILogger<UsersController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _authService.GetAllUsersAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,IsActive")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var result = await _authService.UpdateUserAsync(id, user);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _authService.DeleteUserAsync(id);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
