using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace GestionTicketClinisys.Controllers
{
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is authenticated via session
        private bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession"));
        }

        private int GetCurrentUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserSession");
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            return 0;
        }

       /* [HttpGet]
        [Route("/MesTaches")]
        public async Task<IActionResult> MesTaches()
        {
            // Check if session exists
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user's tasks
            var tasks = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Set user info in ViewBag
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserId = userId;

            return View("GestionTaches", tasks);
        }*/
      /* [HttpGet]
       [Route("/MesTaches")]
       public async Task<IActionResult> MesTaches()
       {
           if (!IsUserAuthenticated())
           {
               return RedirectToAction("Login", "Account");
           }

           var userId = GetCurrentUserId();
           var userName = HttpContext.Session.GetString("UserName") ?? "User";

           var tasks = await _context.TaskItems
               .Include(t => t.Ticket)
               .Include(t => t.AssignedUser)
               .Where(t => t.AssignedUserId == userId)
               .OrderByDescending(t => t.CreatedAt)
               .ToListAsync();

           ViewBag.UserName = userName;
           return View("TaskDashboard", tasks);
       }
        // GET: TaskItems/Create
        public IActionResult Create()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // POST: TaskItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,TicketId,AssignedUserId")] TaskItem taskItem)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                taskItem.CreatedAt = DateTime.UtcNow;
                taskItem.Status = TicketStatus.Open;
                
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("MesTaches");
            }
            ViewBag.Users = _context.Users.ToList();
            return View(taskItem);
        }

        // POST: TaskItems/UpdateStatus/5
        /*[HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, TicketStatus newStatus)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return Json(new { success = false, message = "Task not found" });
            }

            try
            {
                taskItem.Status = newStatus;
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }*/
      [HttpGet]
      [Route("/MesTaches")]
      public async Task<IActionResult> MesTaches()
      {
          if (!IsUserAuthenticated())
          {
              return RedirectToAction("Login", "Account");
          }

          var userId = GetCurrentUserId();
    
          // Get tasks as strings first
          var taskData = await _context.TaskItems
              .Where(t => t.AssignedUserId == userId)
              .Select(t => new {
                  t.Id,
                  t.Title,
                  t.Description,
                  Status = (int)t.Status, // Get as int
                  t.CreatedAt,
                  t.TicketId
              })
              .ToListAsync();

          // Convert to enum when creating the view model
          var tasks = taskData.Select(t => new TaskItem {
              Id = t.Id,
              Title = t.Title,
              Description = t.Description,
              Status = (TicketStatus)t.Status, // Convert int to enum
              CreatedAt = t.CreatedAt,
              TicketId = t.TicketId
          }).ToList();

          ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? "User";
          return View("TaskDashboard", tasks);
      }
        [HttpPost]
        /*public async Task<IActionResult> UpdateStatus(int id, TicketStatus newStatus)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.Status = newStatus;
            _context.Update(taskItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("MesTaches");
        }*/
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int newStatus) // Accept int
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            var taskItem = await _context.TaskItems.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }

            // Convert int to enum
            taskItem.Status = (TicketStatus)newStatus;
    
            _context.Update(taskItem);
            await _context.SaveChangesAsync();
    
            return RedirectToAction("MesTaches");
        }
        // API endpoint to get tasks data for AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            if (!IsUserAuthenticated())
            {
                return Unauthorized();
            }

            var userId = GetCurrentUserId();
            var tasks = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.Description,
                    Status = t.Status.ToString(),
                    t.CreatedAt,
                    TicketId = t.Ticket.Id,
                    TicketTitle = t.Ticket.Title
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Json(tasks);
        }

        // New endpoint to get user info for the dashboard
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            if (!IsUserAuthenticated())
            {
                return Unauthorized();
            }

            var userId = GetCurrentUserId();
            var userName = HttpContext.Session.GetString("UserName");

            return Json(new { 
                userId = userId, 
                userName = userName ?? "User" 
            });
        }

        // API endpoint to get task details
        [HttpGet]
        public async Task<IActionResult> GetTaskDetails(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Unauthorized();
            }

            var userId = GetCurrentUserId();
            var task = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.AssignedUser)
                .Where(t => t.Id == id && t.AssignedUserId == userId)
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.Description,
                    Status = t.Status.ToString(),
                    t.CreatedAt,
                    TicketId = t.Ticket.Id,
                    TicketTitle = t.Ticket.Title,
                    AssignedUser = t.AssignedUser.UserName
                })
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound();
            }

            return Json(task);
        }

        // API endpoint to search tasks
        [HttpGet]
        public async Task<IActionResult> SearchTasks(string query)
        {
            if (!IsUserAuthenticated())
            {
                return Unauthorized();
            }

            var userId = GetCurrentUserId();
            var tasks = await _context.TaskItems
                .Include(t => t.Ticket)
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId && 
                           (t.Title.Contains(query) || 
                            t.Description.Contains(query) || 
                            t.Ticket.Title.Contains(query)))
                .Select(t => new {
                    t.Id,
                    t.Title,
                    t.Description,
                    Status = t.Status.ToString(),
                    t.CreatedAt,
                    TicketId = t.Ticket.Id,
                    TicketTitle = t.Ticket.Title
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Json(tasks);
        }
    }
}