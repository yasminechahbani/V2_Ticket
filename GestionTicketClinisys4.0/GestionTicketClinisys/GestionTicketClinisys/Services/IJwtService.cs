using GestionTicketClinisys.Models;

namespace GestionTicketClinisys.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetUserNameFromToken(string token);
    }
}
