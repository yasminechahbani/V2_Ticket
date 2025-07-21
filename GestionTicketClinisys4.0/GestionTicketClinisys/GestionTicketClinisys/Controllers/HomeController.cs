using System.Diagnostics;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketClinisys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IDataSeedingService _dataSeedingService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IAuthService authService, IJwtService jwtService, IDataSeedingService dataSeedingService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
            _jwtService = jwtService;
            _dataSeedingService = dataSeedingService;
        }

        public IActionResult Index()
        {
            // Check if user is already authenticated
            if (IsUserAuthenticated())
            {
                return RedirectToAction("Menu");
            }

            // Clear any previous login errors
            TempData["LoginError"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginModel = new LoginViewModel
                {
                    UserName = username,
                    Password = password,
                    RememberMe = false // You can add a checkbox for this later
                };

                var result = await _authService.LoginAsync(loginModel);

                if (result.Success && result.Token != null && result.User != null)
                {
                    // Set session data
                    HttpContext.Session.SetString("AuthToken", result.Token);
                    HttpContext.Session.SetString("UserId", result.User.Id.ToString());
                    HttpContext.Session.SetString("UserName", result.User.UserName);
                    HttpContext.Session.SetString("UserRole", result.User.Role);
                    HttpContext.Session.SetString("UserEmail", result.User.Email);

                    _logger.LogInformation($"User {result.User.UserName} logged in successfully");

                    return RedirectToAction("Menu");
                }
                else
                {
                    TempData["LoginError"] = result.Message;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                TempData["LoginError"] = "Une erreur est survenue lors de la connexion.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Menu()
        {
            // Check if user is authenticated
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index");
            }

            // Pass user info to the view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View();
        }

        // Logout action
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            _logger.LogInformation("User logged out");

            return RedirectToAction("Index");
        }

        // Comprehensive data seeding methods
        public async Task<IActionResult> SeedAllData()
        {
            try
            {
                await _dataSeedingService.SeedAllDataAsync();
                return Json(new {
                    success = true,
                    message = "Toutes les données ont été créées avec succès! Inclus: 8 utilisateurs, 12 clients, 8 équipes, 18 modules, et 50 tickets."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding all data");
                return Json(new {
                    success = false,
                    message = $"Erreur lors de la création des données: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                await _dataSeedingService.ClearAllDataAsync();
                return Json(new {
                    success = true,
                    message = "Toutes les données ont été supprimées avec succès!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all data");
                return Json(new {
                    success = false,
                    message = $"Erreur lors de la suppression des données: {ex.Message}"
                });
            }
        }

        // UpdatePasswordHashes method removed - no longer needed

        #region Helper Methods

        private bool IsUserAuthenticated()
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token))
                return false;

            return _jwtService.ValidateToken(token);
        }

        private int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            return null;
        }

        private string? GetCurrentUserName()
        {
            return HttpContext.Session.GetString("UserName");
        }

        #endregion
    }
}
    