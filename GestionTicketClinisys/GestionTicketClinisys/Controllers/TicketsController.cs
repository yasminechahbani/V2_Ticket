using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;

namespace GestionTicketClinisys.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets.Include(t => t.Client).Include(t => t.Module).Include(t => t.Team).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/GestionDemandes
        public async Task<IActionResult> GestionDemandes()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.Module)
                .Include(t => t.Team)
                .Include(t => t.User)
                .ToListAsync();
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.Module)
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,ClientId,UserId,TeamId,ModuleId,ProviderType,DurationDays,DurationMonths,Type,FaultType,Designation,RichTextComment")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.CreationDate = DateTime.Now;
                ticket.Status = TicketStatus.Open;
                ticket.Priority = TicketPriority.Low;
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(ticket);
            return View(ticket);
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
                var userIdStr = Request.Form["UserId"].ToString();
                var teamIdStr = Request.Form["TeamId"].ToString();
                var moduleIdStr = Request.Form["ModuleId"].ToString();

                // Log received data for debugging
                Console.WriteLine($"Received form data: Title={title}, Description={description}, ClientId={clientIdStr}");

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

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    ticket.UserId = userId;
                }

                if (!string.IsNullOrEmpty(teamIdStr) && int.TryParse(teamIdStr, out int teamId))
                {
                    ticket.TeamId = teamId;
                }

                if (!string.IsNullOrEmpty(moduleIdStr) && int.TryParse(moduleIdStr, out int moduleId))
                {
                    ticket.ModuleId = moduleId;
                }

                Console.WriteLine($"Creating ticket: {ticket.Title} for client {ticket.ClientId}");

                _context.Add(ticket);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Ticket created successfully with ID: {ticket.Id}");
                return Json(new { success = true, message = "Demande créée avec succès!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateModal: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new { success = false, errors = new[] { $"Server error: {ex.Message}" } });
            }
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            await PopulateDropdownsAsync(ticket);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,ClientId,UserId,TeamId,ModuleId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            var originalTicket = await _context.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (originalTicket == null)
            {
                return NotFound();
            }
            ticket.CreationDate = originalTicket.CreationDate;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExistsAsync(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(ticket);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.Module)
                .Include(t => t.Team)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TicketExistsAsync(int id)
        {
            return await _context.Tickets.AnyAsync(e => e.Id == id);
        }

        private async Task PopulateDropdownsAsync(object selectedTicket = null)
        {
            if (selectedTicket is Ticket ticket)
            {
                ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "Name", ticket.ClientId);
                ViewData["ModuleId"] = new SelectList(await _context.Modules.ToListAsync(), "Id", "Name", ticket.ModuleId);
                ViewData["TeamId"] = new SelectList(await _context.Teams.ToListAsync(), "Id", "Name", ticket.TeamId);
                ViewData["UserId"] = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", ticket.UserId);
            }
            else
            {
                ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "Id", "Name");
                ViewData["ModuleId"] = new SelectList(await _context.Modules.ToListAsync(), "Id", "Name");
                ViewData["TeamId"] = new SelectList(await _context.Teams.ToListAsync(), "Id", "Name");
                ViewData["UserId"] = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName");
            }
        }

        // GET: Tickets/GetDropdownData - For AJAX requests
        [HttpGet]
        public async Task<IActionResult> GetDropdownData()
        {
            var data = new
            {
                clients = await _context.Clients.Select(c => new { id = c.Id, name = c.Name }).ToListAsync(),
                users = await _context.Users.Select(u => new { id = u.Id, name = u.UserName }).ToListAsync(),
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
                    .Include(t => t.User)
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
                    userId = ticket.UserId,
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
                var userIdStr = Request.Form["UserId"].ToString();
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

                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    ticket.UserId = userId;
                }
                else
                {
                    ticket.UserId = null;
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
    }
}
