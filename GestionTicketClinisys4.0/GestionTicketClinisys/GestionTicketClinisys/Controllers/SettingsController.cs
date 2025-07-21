using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Services;

namespace GestionTicketClinisys.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public SettingsController(ApplicationDbContext context, IJwtService jwtService, IAuthService authService)
        {
            _context = context;
            _jwtService = jwtService;
            _authService = authService;
        }

        // GET: Settings
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

        #region Users Management

        // GET: Settings/GetUsers
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var users = await _context.Users
                    .Include(u => u.Team)
                    .Select(u => new
                    {
                        id = u.Id,
                        userName = u.UserName,
                        email = u.Email,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        role = u.Role,
                        department = u.Department,
                        position = u.Position,
                        teamName = u.Team != null ? u.Team.Name : "",
                        isActive = u.IsActive,
                        lastLogin = u.LastLogin
                    })
                    .ToListAsync();

                return Json(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Settings/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                // Check if username already exists
                if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
                {
                    return Json(new { success = false, message = "Ce nom d'utilisateur existe déjà" });
                }

                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = _authService.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role,
                    Department = request.Department,
                    Position = request.Position,
                    Phone = request.Phone,
                    TeamId = request.TeamId > 0 ? request.TeamId : null,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Utilisateur créé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // PUT: Settings/UpdateUser
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var user = await _context.Users.FindAsync(request.Id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Utilisateur non trouvé" });
                }

                user.Email = request.Email;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Role = request.Role;
                user.Department = request.Department;
                user.Position = request.Position;
                user.Phone = request.Phone;
                user.TeamId = request.TeamId > 0 ? request.TeamId : null;
                user.IsActive = request.IsActive;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Utilisateur mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // DELETE: Settings/DeleteUser
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Utilisateur non trouvé" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Utilisateur supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        #endregion

        #region Teams Management

        // GET: Settings/GetTeams
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var teams = await _context.Teams
                    .Select(t => new
                    {
                        id = t.Id,
                        name = t.Name,
                        userCount = t.Users.Count()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = teams });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Settings/CreateTeam
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var team = new Team { Name = request.Name };
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Équipe créée avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // PUT: Settings/UpdateTeam
        [HttpPut]
        public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var team = await _context.Teams.FindAsync(request.Id);
                if (team == null)
                {
                    return Json(new { success = false, message = "Équipe non trouvée" });
                }

                team.Name = request.Name;
                _context.Teams.Update(team);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Équipe mise à jour avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // DELETE: Settings/DeleteTeam
        [HttpDelete]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null)
                {
                    return Json(new { success = false, message = "Équipe non trouvée" });
                }

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Équipe supprimée avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        #endregion

        #region Clients Management

        // GET: Settings/GetClients
        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var clients = await _context.Clients
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        email = c.Email,
                        ticketCount = c.Tickets.Count()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = clients });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Settings/CreateClient
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var client = new Client
                {
                    Name = request.Name,
                    Email = request.Email
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Client créé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // PUT: Settings/UpdateClient
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var client = await _context.Clients.FindAsync(request.Id);
                if (client == null)
                {
                    return Json(new { success = false, message = "Client non trouvé" });
                }

                client.Name = request.Name;
                client.Email = request.Email;
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Client mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // DELETE: Settings/DeleteClient
        [HttpDelete]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return Json(new { success = false, message = "Client non trouvé" });
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Client supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        #endregion

        #region Modules Management

        // GET: Settings/GetModules
        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var modules = await _context.Modules
                    .Select(m => new
                    {
                        id = m.Id,
                        name = m.Name,
                        ticketCount = m.Tickets.Count()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = modules });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // POST: Settings/CreateModule
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var module = new Module { Name = request.Name };
                _context.Modules.Add(module);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Module créé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // PUT: Settings/UpdateModule
        [HttpPut]
        public async Task<IActionResult> UpdateModule([FromBody] UpdateModuleRequest request)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var module = await _context.Modules.FindAsync(request.Id);
                if (module == null)
                {
                    return Json(new { success = false, message = "Module non trouvé" });
                }

                module.Name = request.Name;
                _context.Modules.Update(module);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Module mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        // DELETE: Settings/DeleteModule
        [HttpDelete]
        public async Task<IActionResult> DeleteModule(int id)
        {
            if (!IsUserAuthenticated())
            {
                return Json(new { success = false, message = "Non authentifié" });
            }

            try
            {
                var module = await _context.Modules.FindAsync(id);
                if (module == null)
                {
                    return Json(new { success = false, message = "Module non trouvé" });
                }

                _context.Modules.Remove(module);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Module supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erreur: {ex.Message}" });
            }
        }

        #endregion

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

        #endregion
    }

    // Request models
    public class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = "User";
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public int TeamId { get; set; }
    }

    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = "User";
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public int TeamId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateTeamRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateClientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateClientRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CreateModuleRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateModuleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
