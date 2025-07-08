using System.Diagnostics;
using GestionTicketClinisys.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionTicketClinisys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Optionally clear login status
            TempData["LoginError"] = null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            if (user != null)
            {
                TempData["IsLoggedIn"] = true;
                TempData["UserName"] = user.UserName;
                return RedirectToAction("Menu");
            }
            else
            {
                TempData["LoginError"] = "Invalid username or password.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Menu()
        {
            return View();
        }

        // Temporary method to seed test data
        public IActionResult SeedTestData()
        {
            // Check if we already have users
            if (!_context.Users.Any())
            {
                // Add test user
                var testUser = new User
                {
                    UserName = "admin",
                    Password = "admin",
                    Email = "admin@test.com",
                    Role = "Administrator"
                };
                _context.Users.Add(testUser);

                // Add test client
                var testClient = new Client
                {
                    Name = "Test Client",
                    Email = "client@test.com"
                };
                _context.Clients.Add(testClient);

                // Add test team
                var testTeam = new Team
                {
                    Name = "Support Team"
                };
                _context.Teams.Add(testTeam);

                // Add test module
                var testModule = new Module
                {
                    Name = "Core System"
                };
                _context.Modules.Add(testModule);

                _context.SaveChanges();
                return Json(new { success = true, message = "Test data created successfully!" });
            }

            return Json(new { success = false, message = "Test data already exists" });
        }
    }
}
    