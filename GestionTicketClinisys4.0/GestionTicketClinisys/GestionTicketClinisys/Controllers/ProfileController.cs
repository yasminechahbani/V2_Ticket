using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public ProfileController(ApplicationDbContext context, IJwtService jwtService, IAuthService authService)
        {
            _context = context;
            _jwtService = jwtService;
            _authService = authService;
        }

        // GET: Profile
        public async Task<IActionResult> Index()
        {
            // Check authentication
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var profileViewModel = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Nationality = user.Nationality,
                Position = user.Position,
                Department = user.Department,
                Manager = user.Manager,
                Role = user.Role ?? "User",
                TeamName = user.Team?.Name,
                TeamId = user.TeamId,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive,
                ProfilePicture = user.ProfilePicture
            };

            return View(profileViewModel);
        }

        // POST: Profile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = "Utilisateur non trouvé" });
                }

                // Update user properties
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Phone = model.Phone;
                user.Address = model.Address;
                user.DateOfBirth = model.DateOfBirth;
                user.Nationality = model.Nationality;
                user.Position = model.Position;
                user.Department = model.Department;
                user.Manager = model.Manager;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Profil mis à jour avec succès!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: Profile/GetRecentActivity
        [HttpGet]
        public async Task<IActionResult> GetRecentActivity()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                // Get recent task activities for this user
                var activities = new List<object>();

                // Get tasks assigned to this user
                var userTasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.Ticket.Client)
                    .Where(t => t.AssignedUserId == userId.Value)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(10)
                    .ToListAsync();

                foreach (var task in userTasks)
                {
                    var activityTitle = "";
                    var activityIcon = "";
                    var activityTime = task.CreatedAt;

                    switch (task.Status)
                    {
                        case Models.Enums.TaskStatus.NotStarted:
                            activityTitle = $"Nouvelle tâche assignée: {task.Title}";
                            activityIcon = "📋";
                            break;
                        case Models.Enums.TaskStatus.InProgress:
                            activityTitle = $"Tâche en cours: {task.Title}";
                            activityIcon = "⚡";
                            break;
                        case Models.Enums.TaskStatus.Completed:
                            activityTitle = $"Tâche terminée: {task.Title}";
                            activityIcon = "✅";
                            break;
                        case Models.Enums.TaskStatus.Cancelled:
                            activityTitle = $"Tâche annulée: {task.Title}";
                            activityIcon = "❌";
                            break;
                    }

                    activities.Add(new
                    {
                        title = activityTitle,
                        time = GetRelativeTime(activityTime),
                        icon = activityIcon,
                        subtitle = $"Demande: {task.Ticket?.Title} - Client: {task.Ticket?.Client?.Name}"
                    });
                }

                // If no tasks, show a helpful message
                if (!activities.Any())
                {
                    activities.Add(new
                    {
                        title = "Aucune tâche assignée",
                        time = "Maintenant",
                        icon = "💼",
                        subtitle = "Vous n'avez actuellement aucune tâche assignée"
                    });
                }

                return Json(new { success = true, activities = activities });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

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

        private string GetRelativeTime(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "À l'instant";
            if (timeSpan.TotalMinutes < 60)
                return $"Il y a {(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")}";
            if (timeSpan.TotalHours < 24)
                return $"Il y a {(int)timeSpan.TotalHours} heure{((int)timeSpan.TotalHours > 1 ? "s" : "")}";
            if (timeSpan.TotalDays < 7)
                return $"Il y a {(int)timeSpan.TotalDays} jour{((int)timeSpan.TotalDays > 1 ? "s" : "")}";
            if (timeSpan.TotalDays < 30)
                return $"Il y a {(int)(timeSpan.TotalDays / 7)} semaine{((int)(timeSpan.TotalDays / 7) > 1 ? "s" : "")}";
            
            return dateTime.ToString("dd/MM/yyyy");
        }

        #endregion
    }
}
