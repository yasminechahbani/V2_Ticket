using GestionTicketClinisys.Models;

namespace GestionTicketClinisys.Services
{
    public interface IDataSeedingService
    {
        Task SeedAllDataAsync();
        Task SeedUsersAsync();
        Task SeedClientsAsync();
        Task SeedTeamsAsync();
        Task SeedModulesAsync();
        Task SeedTicketsAsync();
        Task ClearAllDataAsync();
    }
}
