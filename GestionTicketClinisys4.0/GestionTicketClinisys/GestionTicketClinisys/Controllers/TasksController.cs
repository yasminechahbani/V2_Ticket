using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public TasksController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: Tasks/MesTaches - Show enhanced tasks dashboard
        public async Task<IActionResult> MesTaches()
        {
            // Check authentication
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get user tasks with enhanced data
            var userTasks = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.Ticket.Client)
                .Include(t => t.Ticket.Team)
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == currentUserId.Value)
                .OrderBy(t => t.KanbanOrder)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Get team members working on same tickets
            var ticketIds = userTasks.Select(t => t.TicketId).Distinct().ToList();
            var teamMembers = await _context.TaskItems
                .Include(t => t.AssignedUser)
                .Where(t => ticketIds.Contains(t.TicketId) && t.AssignedUserId != currentUserId.Value)
                .Select(t => t.AssignedUser)
                .Distinct()
                .ToListAsync();

            // Pass data to view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.TeamMembers = teamMembers;
            ViewBag.CurrentUserId = currentUserId.Value;

            return View(userTasks);
        }

        // GET: Tasks/TestKanban - Test Kanban page
        public async Task<IActionResult> TestKanban()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.Ticket.Client)
                    .Include(t => t.AssignedUser)
                    .Where(t => t.AssignedUserId == currentUserId.Value)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return View(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TestKanban: {ex}");
                return View(new List<TaskItem>());
            }
        }

        // GET: Tasks/GetUserTasks - AJAX endpoint for user tasks
        [HttpGet]
        public async Task<IActionResult> GetUserTasks()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.Ticket.Client)
                    .Where(t => t.AssignedUserId == currentUserId.Value)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = t.Description,
                        status = t.Status.ToString(),
                        createdAt = t.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                        ticketTitle = t.Ticket.Title,
                        clientName = t.Ticket.Client.Name,
                        ticketId = t.TicketId
                    })
                    .ToListAsync();

                return Json(new { success = true, data = tasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Tasks/UpdateTaskStatus - Update task status
        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                var task = await _context.TaskItems
                    .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.AssignedUserId == currentUserId.Value);

                if (task == null)
                {
                    return Json(new { success = false, message = "Tâche non trouvée ou non autorisée" });
                }

                if (Enum.TryParse<Models.Enums.TaskStatus>(request.Status, out var newStatus))
                {
                    task.Status = newStatus;
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Statut mis à jour avec succès" });
                }
                else
                {
                    return Json(new { success = false, message = "Statut invalide" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Tasks/CreateTask - Create new task (for admin/manager)
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission to create tasks (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
            }

            try
            {
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

        // GET: Tasks/GetTasksForTicket - Get tasks for a specific ticket
        [HttpGet]
        public async Task<IActionResult> GetTasksForTicket(int ticketId)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.AssignedUser)
                    .Where(t => t.TicketId == ticketId)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = t.Description,
                        status = t.Status.ToString(),
                        createdAt = t.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                        assignedUserName = t.AssignedUser.UserName,
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

        // POST: Tasks/UpdateKanbanOrder - Update task order in Kanban
        [HttpPost]
        public async Task<IActionResult> UpdateKanbanOrder([FromBody] UpdateKanbanOrderRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                // Validate request
                if (request == null)
                {
                    return Json(new { success = false, message = "Requête invalide" });
                }

                if (request.TaskId <= 0)
                {
                    return Json(new { success = false, message = "ID de tâche invalide" });
                }

                if (string.IsNullOrEmpty(request.NewStatus))
                {
                    return Json(new { success = false, message = "Statut invalide" });
                }

                // Convert string status to enum
                if (!Enum.TryParse<Models.Enums.TaskStatus>(request.NewStatus, true, out var newStatus))
                {
                    return Json(new { success = false, message = $"Statut non reconnu: {request.NewStatus}" });
                }

                var task = await _context.TaskItems.FindAsync(request.TaskId);
                if (task == null)
                {
                    return Json(new { success = false, message = "Tâche non trouvée" });
                }

                // Update status and order
                task.Status = newStatus;
                task.KanbanOrder = request.NewOrder;

                // Update completion date if task is completed
                if (newStatus == Models.Enums.TaskStatus.Completed && task.CompletedDate == null)
                {
                    task.CompletedDate = DateTime.Now;
                    task.Progress = 100;
                }
                else if (newStatus != Models.Enums.TaskStatus.Completed)
                {
                    task.CompletedDate = null;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tâche mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                Console.WriteLine($"Error in UpdateKanbanOrder: {ex}");
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: Tasks/GetKanbanData - Get tasks organized for Kanban board
        [HttpGet]
        public async Task<IActionResult> GetKanbanData()
        {
            Console.WriteLine("GetKanbanData called");

            if (!IsUserAuthenticated())
            {
                Console.WriteLine("User not authenticated");
                return Json(new { success = false, message = "Non authentifié" });
            }

            var currentUserId = GetCurrentUserId();
            Console.WriteLine($"Current user ID: {currentUserId}");

            if (currentUserId == null)
            {
                Console.WriteLine("User ID is null");
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.Ticket.Client)
                    .Include(t => t.AssignedUser)
                    .Where(t => t.AssignedUserId == currentUserId.Value)
                    .OrderBy(t => t.KanbanOrder)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        description = t.Description,
                        status = t.Status.ToString(),
                        priority = t.Priority,
                        progress = t.Progress,
                        dueDate = t.DueDate != null ? t.DueDate.Value.ToString("yyyy-MM-dd") : null,
                        estimatedHours = t.EstimatedHours,
                        actualHours = t.ActualHours,
                        ticketTitle = t.Ticket != null ? t.Ticket.Title : "No ticket",
                        clientName = t.Ticket != null && t.Ticket.Client != null ? t.Ticket.Client.Name : "No client",
                        assignedUserName = t.AssignedUser != null ? t.AssignedUser.UserName : "No user",
                        kanbanOrder = t.KanbanOrder
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {tasks.Count} tasks for user {currentUserId}");

                var kanbanData = new
                {
                    notStarted = tasks.Where(t => t.status == "NotStarted").ToList(),
                    inProgress = tasks.Where(t => t.status == "InProgress").ToList(),
                    completed = tasks.Where(t => t.status == "Completed").ToList(),
                    cancelled = tasks.Where(t => t.status == "Cancelled").ToList()
                };

                Console.WriteLine($"Kanban data: NotStarted={kanbanData.notStarted.Count}, InProgress={kanbanData.inProgress.Count}, Completed={kanbanData.completed.Count}, Cancelled={kanbanData.cancelled.Count}");

                return Json(new { success = true, data = kanbanData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetKanbanData: {ex}");
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: Tasks/GetGanttData - Get tasks for Gantt chart
        [HttpGet]
        public async Task<IActionResult> GetGanttData()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                var tasks = await _context.TaskItems
                    .Include(t => t.Ticket)
                    .Include(t => t.AssignedUser)
                    .Where(t => t.AssignedUserId == currentUserId.Value)
                    .Select(t => new
                    {
                        id = t.Id,
                        title = t.Title,
                        start = t.StartDate != null ? t.StartDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                        end = t.DueDate != null ? t.DueDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.AddDays(7).ToString("yyyy-MM-dd"),
                        progress = t.Progress,
                        status = t.Status.ToString(),
                        priority = t.Priority,
                        estimatedHours = t.EstimatedHours,
                        actualHours = t.ActualHours,
                        ticketTitle = t.Ticket.Title,
                        assignedUserName = t.AssignedUser.UserName
                    })
                    .ToListAsync();

                return Json(new { success = true, data = tasks });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // GET: Tasks/GetTeamMembers - Get team members working on same projects
        [HttpGet]
        public async Task<IActionResult> GetTeamMembers()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Json(new { success = false, message = "Utilisateur non trouvé" });
            }

            try
            {
                // Get tickets that current user is working on
                var userTicketIds = await _context.TaskItems
                    .Where(t => t.AssignedUserId == currentUserId.Value)
                    .Select(t => t.TicketId)
                    .Distinct()
                    .ToListAsync();

                // Get other users working on same tickets
                var teamMembers = await _context.TaskItems
                    .Include(t => t.AssignedUser)
                    .Include(t => t.Ticket)
                    .Where(t => userTicketIds.Contains(t.TicketId) && t.AssignedUserId != currentUserId.Value)
                    .GroupBy(t => t.AssignedUser)
                    .Select(g => new
                    {
                        userId = g.Key.Id,
                        userName = g.Key.UserName,
                        fullName = $"{g.Key.FirstName} {g.Key.LastName}",
                        email = g.Key.Email,
                        role = g.Key.Role,
                        tasksCount = g.Count(),
                        sharedTickets = g.Select(t => t.Ticket.Title).Distinct().ToList(),
                        lastLogin = g.Key.LastLogin != null ? g.Key.LastLogin.Value.ToString("dd/MM/yyyy HH:mm") : null
                    })
                    .ToListAsync();

                return Json(new { success = true, data = teamMembers });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Tasks/UpdateTask - Update task details
        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDetailsRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                // Validate request
                if (request == null)
                {
                    return Json(new { success = false, message = "Requête invalide" });
                }

                if (request.Id <= 0)
                {
                    return Json(new { success = false, message = "ID de tâche invalide" });
                }

                if (string.IsNullOrEmpty(request.Title))
                {
                    return Json(new { success = false, message = "Le titre est requis" });
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    return Json(new { success = false, message = "La description est requise" });
                }

                // Convert string status to enum
                if (!Enum.TryParse<Models.Enums.TaskStatus>(request.Status, true, out var status))
                {
                    return Json(new { success = false, message = $"Statut non reconnu: {request.Status}" });
                }

                var task = await _context.TaskItems.FindAsync(request.Id);
                if (task == null)
                {
                    return Json(new { success = false, message = "Tâche non trouvée" });
                }

                // Update task properties
                task.Title = request.Title;
                task.Description = request.Description;
                task.Status = status;
                task.Priority = request.Priority;
                task.Progress = request.Progress;
                task.EstimatedHours = request.EstimatedHours;
                task.ActualHours = request.ActualHours;

                // Update due date if provided
                if (!string.IsNullOrEmpty(request.DueDate))
                {
                    if (DateTime.TryParse(request.DueDate, out var dueDate))
                    {
                        task.DueDate = dueDate;
                    }
                }
                else
                {
                    task.DueDate = null;
                }

                // Update completion date if task is completed
                if (status == Models.Enums.TaskStatus.Completed && task.CompletedDate == null)
                {
                    task.CompletedDate = DateTime.Now;
                    if (task.Progress < 100)
                    {
                        task.Progress = 100;
                    }
                }
                else if (status != Models.Enums.TaskStatus.Completed)
                {
                    task.CompletedDate = null;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Tâche mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTask: {ex}");
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
            var userIdString = HttpContext.Session.GetString("UserId");
            if (int.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return null;
        }

        #endregion
    }

    // Request models
    public class UpdateTaskStatusRequest
    {
        public int TaskId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateKanbanOrderRequest
    {
        public int TaskId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public int NewOrder { get; set; }
    }

    public class UpdateTaskDetailsRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Priority { get; set; } = 2;
        public int Progress { get; set; } = 0;
        public string DueDate { get; set; } = string.Empty;
        public int EstimatedHours { get; set; } = 0;
        public int ActualHours { get; set; } = 0;
    }
}
