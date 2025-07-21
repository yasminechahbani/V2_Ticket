using Microsoft.AspNetCore.Mvc;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class GuideController : Controller
    {
        private readonly IJwtService _jwtService;

        public GuideController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        // GET: Guide/Index - Main user guide page
        public IActionResult Index()
        {
            // Check authentication
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            // Pass user info to view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View();
        }

        #region Helper Methods

        private bool IsUserAuthenticated()
        {
            var token = HttpContext.Session.GetString("AuthToken");

            if (string.IsNullOrEmpty(token))
                return false;

            return _jwtService.ValidateToken(token);
        }

        #endregion
    }
}
