using Microsoft.AspNetCore.Mvc;
using PRM.Dtos;
using PRM.Services;

namespace PRM.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDtos registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(registerDto);
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDtos loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                HttpContext.Session.SetInt32("UserId", result.User.Id);
                HttpContext.Session.SetString("UserEmail", result.User.Email);
                HttpContext.Session.SetString("UserName", $"{result.User.FirstName} {result.User.LastName}");
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.Message);
            return View(loginDto);
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Logged out successfully";
            return RedirectToAction("Index", "Home");
        }

        // GET: Auth/Profile
        public async Task<IActionResult> Profile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
