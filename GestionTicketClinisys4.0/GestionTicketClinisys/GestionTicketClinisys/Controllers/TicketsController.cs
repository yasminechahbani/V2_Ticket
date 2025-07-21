using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public TicketsController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // GET: Tickets - Redirect to GestionDemandes
        public IActionResult Index()
        {
            return RedirectToAction(nameof(GestionDemandes));
        }

        // GET: Tickets/GestionDemandes
        public async Task<IActionResult> GestionDemandes()
        {
            // Check authentication
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            var tickets = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.Module)
                .Include(t => t.Team)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();

            // Pass user info to view
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            return View(tickets);
        }

        // Details functionality is handled by the edit modal in GestionDemandes

        // Create functionality is handled by the modal in GestionDemandes

        // GET: Tickets/GetTasksForTicket - Get tasks for a specific ticket (for admin/manager)
        [HttpGet]
        public async Task<IActionResult> GetTasksForTicket(int ticketId)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            // Check if user has permission to view tasks (admin/manager)
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && userRole != "Manager")
            {
                return Json(new { success = false, message = "Accès non autorisé" });
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

        // POST: Tickets/CreateTaskForTicket - Create new task for a ticket (for admin/manager)
        [HttpPost]
        public async Task<IActionResult> CreateTaskForTicket([FromBody] CreateTaskForTicketRequest request)
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

        // GET: Tickets/GetUsersForAssignment - Get users for task assignment
        [HttpGet]
        public async Task<IActionResult> GetUsersForAssignment()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var users = await _context.Users
                    .Where(u => u.IsActive)
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        fullName = $"{u.FirstName} {u.LastName}".Trim()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Tickets/CreateModal - For modal form submission
        [HttpPost]
        public async Task<IActionResult> CreateModal()
        {
            try
            {
                // Get form data manually to handle conversion properly
                var title = Request.Form["Title"].ToString();
                var description = Request.Form["Description"].ToString();
                var clientIdStr = Request.Form["ClientId"].ToString();
                var richTextComment = Request.Form["RichTextComment"].ToString();

                // Optional fields
                var typeStr = Request.Form["Type"].ToString();
                var statusStr = Request.Form["Status"].ToString();
                var priorityStr = Request.Form["Priority"].ToString();
                var durationDaysStr = Request.Form["DurationDays"].ToString();
                var durationMonthsStr = Request.Form["DurationMonths"].ToString();

                var teamIdStr = Request.Form["TeamId"].ToString();
                var moduleIdStr = Request.Form["ModuleId"].ToString();

                // Validate form data

                // Validate required fields
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(clientIdStr))
                {
                    return Json(new { success = false, errors = new[] { "Title, Description, and Client are required" } });
                }

                if (!int.TryParse(clientIdStr, out int clientId) || clientId <= 0)
                {
                    return Json(new { success = false, errors = new[] { "Valid Client selection is required" } });
                }

                // Create ticket with proper data conversion
                var ticket = new Ticket
                {
                    Title = title,
                    Description = description,
                    ClientId = clientId,
                    RichTextComment = richTextComment,
                    CreationDate = DateTime.Now,
                    Status = TicketStatus.Open,
                    Priority = TicketPriority.Low
                };

                // Handle optional enum fields
                if (!string.IsNullOrEmpty(typeStr) && int.TryParse(typeStr, out int typeInt))
                {
                    ticket.Type = (TicketType)typeInt;
                }

                if (!string.IsNullOrEmpty(statusStr) && int.TryParse(statusStr, out int statusInt))
                {
                    ticket.Status = (TicketStatus)statusInt;
                }

                if (!string.IsNullOrEmpty(priorityStr) && int.TryParse(priorityStr, out int priorityInt))
                {
                    ticket.Priority = (TicketPriority)priorityInt;
                }

                // Handle optional integer fields
                if (!string.IsNullOrEmpty(durationDaysStr) && int.TryParse(durationDaysStr, out int durationDays))
                {
                    ticket.DurationDays = durationDays;
                }

                if (!string.IsNullOrEmpty(durationMonthsStr) && int.TryParse(durationMonthsStr, out int durationMonths))
                {
                    ticket.DurationMonths = durationMonths;
                }



                if (!string.IsNullOrEmpty(teamIdStr) && int.TryParse(teamIdStr, out int teamId))
                {
                    ticket.TeamId = teamId;
                }

                if (!string.IsNullOrEmpty(moduleIdStr) && int.TryParse(moduleIdStr, out int moduleId))
                {
                    ticket.ModuleId = moduleId;
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Demande créée avec succès!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { $"Server error: {ex.Message}" } });
            }
        }

        // Edit functionality is handled by the EditModal method

        // Delete functionality is handled by the DeleteModal and BulkDelete methods

        private async Task<bool> TicketExistsAsync(int id)
        {
            return await _context.Tickets.AnyAsync(e => e.Id == id);
        }

        private async Task PopulateDropdownsAsync(object? selectedTicket = null)
        {
            if (selectedTicket is Ticket ticket)
            {
                ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "Name", ticket.ClientId);
                ViewData["ModuleId"] = new SelectList(await _context.Modules.ToListAsync(), "Id", "Name", ticket.ModuleId);
                ViewData["TeamId"] = new SelectList(await _context.Teams.ToListAsync(), "Id", "Name", ticket.TeamId);

            }
            else
            {
                ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "Name");
                ViewData["ModuleId"] = new SelectList(await _context.Modules.ToListAsync(), "Id", "Name");
                ViewData["TeamId"] = new SelectList(await _context.Teams.ToListAsync(), "Id", "Name");

            }
        }

        // GET: Tickets/GetDropdownData - For AJAX requests
        [HttpGet]
        public async Task<IActionResult> GetDropdownData()
        {
            var data = new
            {
                clients = await _context.Clients.Select(c => new { id = c.Id, name = c.Name }).ToListAsync(),
                teams = await _context.Teams.Select(t => new { id = t.Id, name = t.Name }).ToListAsync(),
                modules = await _context.Modules.Select(m => new { id = m.Id, name = m.Name }).ToListAsync()
            };
            return Json(data);
        }

        // GET: Tickets/GetTicketData/5 - For edit modal
        [HttpGet]
        public async Task<IActionResult> GetTicketData(int id)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Client)
                    .Include(t => t.Module)
                    .Include(t => t.Team)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (ticket == null)
                {
                    return Json(new { success = false, message = "Ticket not found" });
                }

                var ticketData = new
                {
                    id = ticket.Id,
                    title = ticket.Title,
                    description = ticket.Description,
                    richTextComment = ticket.RichTextComment,
                    clientId = ticket.ClientId,

                    teamId = ticket.TeamId,
                    moduleId = ticket.ModuleId,
                    type = ticket.Type,
                    durationDays = ticket.DurationDays,
                    durationMonths = ticket.DurationMonths,
                    status = ticket.Status,
                    priority = ticket.Priority
                };

                return Json(new { success = true, ticket = ticketData });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ticket data: {ex.Message}");
                return Json(new { success = false, message = "Error retrieving ticket data" });
            }
        }

        // POST: Tickets/EditModal - For modal edit submission
        [HttpPost]
        public async Task<IActionResult> EditModal()
        {
            try
            {
                var idStr = Request.Form["Id"].ToString();
                if (!int.TryParse(idStr, out int ticketId))
                {
                    return Json(new { success = false, errors = new[] { "Invalid ticket ID" } });
                }

                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return Json(new { success = false, errors = new[] { "Ticket not found" } });
                }

                // Get form data
                var title = Request.Form["Title"].ToString();
                var description = Request.Form["Description"].ToString();
                var clientIdStr = Request.Form["ClientId"].ToString();
                var richTextComment = Request.Form["RichTextComment"].ToString();

                Console.WriteLine($"Editing ticket {ticketId}: Title={title}, Description={description}, ClientId={clientIdStr}");

                // Validate required fields
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(clientIdStr))
                {
                    return Json(new { success = false, errors = new[] { "Title, Description, and Client are required" } });
                }

                if (!int.TryParse(clientIdStr, out int clientId) || clientId <= 0)
                {
                    return Json(new { success = false, errors = new[] { "Valid Client selection is required" } });
                }

                // Update ticket properties
                ticket.Title = title;
                ticket.Description = description;
                ticket.ClientId = clientId;
                ticket.RichTextComment = richTextComment;

                // Handle optional fields (same logic as create)
                var typeStr = Request.Form["Type"].ToString();
                var statusStr = Request.Form["Status"].ToString();
                var priorityStr = Request.Form["Priority"].ToString();
                var durationDaysStr = Request.Form["DurationDays"].ToString();
                var durationMonthsStr = Request.Form["DurationMonths"].ToString();

                var teamIdStr = Request.Form["TeamId"].ToString();
                var moduleIdStr = Request.Form["ModuleId"].ToString();

                // Update optional enum fields
                if (!string.IsNullOrEmpty(typeStr) && int.TryParse(typeStr, out int typeInt))
                {
                    ticket.Type = (TicketType)typeInt;
                }
                else
                {
                    ticket.Type = null;
                }

                if (!string.IsNullOrEmpty(statusStr) && int.TryParse(statusStr, out int statusInt))
                {
                    ticket.Status = (TicketStatus)statusInt;
                }

                if (!string.IsNullOrEmpty(priorityStr) && int.TryParse(priorityStr, out int priorityInt))
                {
                    ticket.Priority = (TicketPriority)priorityInt;
                }

                // Update optional integer fields
                if (!string.IsNullOrEmpty(durationDaysStr) && int.TryParse(durationDaysStr, out int durationDays))
                {
                    ticket.DurationDays = durationDays;
                }
                else
                {
                    ticket.DurationDays = null;
                }

                if (!string.IsNullOrEmpty(durationMonthsStr) && int.TryParse(durationMonthsStr, out int durationMonths))
                {
                    ticket.DurationMonths = durationMonths;
                }
                else
                {
                    ticket.DurationMonths = null;
                }



                if (!string.IsNullOrEmpty(teamIdStr) && int.TryParse(teamIdStr, out int teamId))
                {
                    ticket.TeamId = teamId;
                }
                else
                {
                    ticket.TeamId = null;
                }

                if (!string.IsNullOrEmpty(moduleIdStr) && int.TryParse(moduleIdStr, out int moduleId))
                {
                    ticket.ModuleId = moduleId;
                }
                else
                {
                    ticket.ModuleId = null;
                }

                _context.Update(ticket);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Ticket {ticketId} updated successfully");
                return Json(new { success = true, message = "Ticket mis à jour avec succès!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in EditModal: {ex.Message}");
                return Json(new { success = false, errors = new[] { $"Server error: {ex.Message}" } });
            }
        }

        // POST: Tickets/DeleteModal - For modal delete confirmation
        [HttpPost]
        public async Task<IActionResult> DeleteModal(int id)
        {
            try
            {
                Console.WriteLine($"Attempting to delete ticket {id}");

                var ticket = await _context.Tickets.FindAsync(id);
                if (ticket == null)
                {
                    return Json(new { success = false, message = "Ticket not found" });
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Ticket {id} deleted successfully");
                return Json(new { success = true, message = "Ticket supprimé avec succès!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteModal: {ex.Message}");
                return Json(new { success = false, message = $"Error deleting ticket: {ex.Message}" });
            }
        }

        // POST: Tickets/BulkDelete - For bulk delete functionality
        [HttpPost]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
        {
            try
            {
                Console.WriteLine($"Attempting to bulk delete {request.TicketIds.Count} tickets");

                if (request.TicketIds == null || !request.TicketIds.Any())
                {
                    return Json(new { success = false, message = "No tickets selected for deletion" });
                }

                var tickets = await _context.Tickets
                    .Where(t => request.TicketIds.Contains(t.Id))
                    .ToListAsync();

                if (!tickets.Any())
                {
                    return Json(new { success = false, message = "No tickets found to delete" });
                }

                Console.WriteLine($"Found {tickets.Count} tickets to delete");

                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully deleted {tickets.Count} tickets");
                return Json(new {
                    success = true,
                    message = $"{tickets.Count} tickets supprimés avec succès!",
                    deletedCount = tickets.Count
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in BulkDelete: {ex.Message}");
                return Json(new { success = false, message = $"Error deleting tickets: {ex.Message}" });
            }
        }

        public class BulkDeleteRequest
        {
            public List<int> TicketIds { get; set; } = new List<int>();
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

        private string? GetCurrentUserName()
        {
            return HttpContext.Session.GetString("UserName");
        }

        #endregion
    }

    // Request models for task management
    public class CreateTaskForTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public int AssignedUserId { get; set; }
    }
}
