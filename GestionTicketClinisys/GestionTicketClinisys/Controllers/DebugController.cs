using Microsoft.AspNetCore.Mvc;

namespace GestionTicketClinisys.Controllers
{
    public class DebugController : Controller
    {
        // Test route to verify routing works
        [HttpGet]
        [Route("/test")]
        public IActionResult Test()
        {
            ViewBag.Message = "Routing works!";
            ViewBag.SessionExists = !string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession"));
            ViewBag.SessionUserId = HttpContext.Session.GetString("UserSession");
            ViewBag.SessionUserName = HttpContext.Session.GetString("UserName");
            
            return View();
        }
    }
}