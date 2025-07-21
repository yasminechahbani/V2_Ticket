using GestionTicketClinisys.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionTicketClinisys.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginViewModel model);
        Task<AuthResponse> ChangePasswordAsync(int userId, ChangePasswordViewModel model);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByUserNameAsync(string userName);
        Task UpdateLastLoginAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}

namespace GestionTicketClinisys.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await GetUserByUserNameAsync(model.UserName);

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Nom d'utilisateur ou mot de passe incorrect"
                    };
                }

                if (!VerifyPassword(model.Password, user.Password))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Nom d'utilisateur ou mot de passe incorrect"
                    };
                }

                // Update last login
                await UpdateLastLoginAsync(user.Id);

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);

                var userProfile = new UserProfileViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email ?? "",
                    Role = user.Role ?? "User",
                    LastLogin = user.LastLogin ?? DateTime.Now,
                    TeamName = user.Team?.Name
                };

                return new AuthResponse
                {
                    Success = true,
                    Message = "Connexion réussie",
                    Token = token,
                    User = userProfile
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Erreur lors de la connexion: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Utilisateur non trouvé"
                    };
                }

                if (!VerifyPassword(model.CurrentPassword, user.Password))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Mot de passe actuel incorrect"
                    };
                }

                user.Password = HashPassword(model.NewPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new AuthResponse
                {
                    Success = true,
                    Message = "Mot de passe modifié avec succès"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Erreur lors du changement de mot de passe: {ex.Message}"
                };
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
