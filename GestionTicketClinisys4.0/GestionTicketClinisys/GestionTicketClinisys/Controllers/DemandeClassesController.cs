using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class DemandeClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public DemandeClassesController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: DemandeClasses/Index - Main task management page for admin/manager
        public async Task<IActionResult> Index()
        {
            // Check authentication
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Session expirée. Veuillez vous reconnecter.";
                return RedirectToAction("Index", "Home");
            }

            // Check if user has permission (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            var userName = HttpContext.Session.GetString("UserName");

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"DemandeClasses Access - User: {userName}, Role: {userRole}");

            if (userRole != "Admin" && userRole != "Administrator" && userRole != "Manager")
            {
                TempData["ErrorMessage"] = $"Accès non autorisé. Votre rôle '{userRole}' ne permet pas d'accéder à cette page. Seuls les administrateurs et managers peuvent accéder à cette page.";
                return RedirectToAction("Menu", "Home");
            }

            var tasks = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.Ticket.Client)
                .Include(t => t.AssignedUser)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Pass user info to view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View(tasks);
        }

        // GET: DemandeClasses/GetAllTasks - AJAX endpoint for all tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Administrator" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.Ticket.Client)
                    .Include(t => t.AssignedUser)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = t.Description,
                        status = t.Status.ToString(),
                        createdAt = t.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                        ticketTitle = t.Ticket.Title,
                        clientName = t.Ticket.Client.Name,
                        assignedUserName = t.AssignedUser.UserName,
                        ticketId = t.TicketId,
                        assignedUserId = t.AssignedUserId
                    })
                    .ToListAsync();

                return Json(new { success = true, data = tasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: DemandeClasses/CreateTask - Create new task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Administrator" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            try
            {
                // Validate that the user belongs to the same team as the ticket
                var ticket = await _context.Tickets
                    .Include(t => t.Team)
                    .FirstOrDefaultAsync(t => t.Id == request.TicketId);

                if (ticket == null)
                {
                    return Json(new { success = false, message = "Demande non trouvée" });
                }

                var assignedUser = await _context.Users
                    .Include(u => u.Team)
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedUserId);

                if (assignedUser == null)
                {
                    return Json(new { success = false, message = "Utilisateur non trouvé" });
                }

                // Check if user belongs to the same team as the ticket
                if (ticket.TeamId.HasValue && assignedUser.TeamId != ticket.TeamId)
                {
                    return Json(new { success = false, message = "L'utilisateur doit appartenir à la même équipe que la demande" });
                }

                var task = new TaskItem
                {
                    Title = request.Title,
                    Description = request.Description,
                    TicketId = request.TicketId,
                    AssignedUserId = request.AssignedUserId,
                    Status = Models.Enums.TaskStatus.NotStarted,
                    CreatedAt = DateTime.Now
                };

                _context.TaskItems.Add(task);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tâche créée avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: DemandeClasses/UpdateTask - Update task
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Administrator" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            try
            {
                var task = await _context.TaskItems.FindAsync(request.Id);
                if (task == null)
                {
                    return Json(new { success = false, message = "Tâche non trouvée" });
                }

                task.Title = request.Title;
                task.Description = request.Description;
                task.AssignedUserId = request.AssignedUserId;
                
                if (Enum.TryParse<Models.Enums.TaskStatus>(request.Status, out var newStatus))
                {
                    task.Status = newStatus;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tâche mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // DELETE: DemandeClasses/DeleteTask - Delete task
        [HttpDelete]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Administrator" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            try
            {
                var task = await _context.TaskItems.FindAsync(id);
                if (task == null)
                {
                    return Json(new { success = false, message = "Tâche non trouvée" });
                }

                _context.TaskItems.Remove(task);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tâche supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: DemandeClasses/GetTicketsForAssignment - Get tickets for task assignment
        [HttpGet]
        public async Task<IActionResult> GetTicketsForAssignment()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var tickets = await _context.Tickets
                    .Include(t => t.Client)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        clientName = t.Client.Name
                    })
                    .ToListAsync();

                return Json(new { success = true, data = tickets });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: DemandeClasses/GetUsersForAssignment - Get users for task assignment
        [HttpGet]
        public async Task<IActionResult> GetUsersForAssignment(int? ticketId = null)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                IQueryable<User> usersQuery = _context.Users.Where(u => u.IsActive);

                // If ticketId is provided, filter users by the ticket's team
                if (ticketId.HasValue)
                {
                    var ticket = await _context.Tickets
                        .Include(t => t.Team)
                        .FirstOrDefaultAsync(t => t.Id == ticketId.Value);

                    if (ticket != null && ticket.TeamId.HasValue)
                    {
                        usersQuery = usersQuery.Where(u => u.TeamId == ticket.TeamId.Value);
                    }
                }

                var users = await usersQuery
                    .Include(u => u.Team)
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        fullName = $"{u.FirstName} {u.LastName}".Trim(),
                        teamName = u.Team != null ? u.Team.Name : "Aucune équipe"
                    })
                    .ToListAsync();

                return Json(new { success = true, data = users });
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

        #endregion
    }

    // Request models
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public int AssignedUserId { get; set; }
    }

    public class UpdateTaskRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AssignedUserId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
