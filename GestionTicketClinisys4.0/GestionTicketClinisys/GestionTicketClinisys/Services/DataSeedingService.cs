using Microsoft.EntityFrameworkCore;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.Models.Enums;

namespace GestionTicketClinisys.Services
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public DataSeedingService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task SeedAllDataAsync()
        {
            // Clear existing data first
            await ClearAllDataAsync();

            // Seed in order (respecting foreign key dependencies)
            await SeedTeamsAsync();
            await SeedUsersAsync();
            await SeedClientsAsync();
            await SeedModulesAsync();
            await SeedTicketsAsync();

            await _context.SaveChangesAsync();
        }

        public async Task SeedTeamsAsync()
        {
            if (await _context.Teams.AnyAsync()) return;

            var teams = new List<Team>
            {
                new Team { Name = "Support Technique" },
                new Team { Name = "Développement" },
                new Team { Name = "Infrastructure" },
                new Team { Name = "Sécurité" },
                new Team { Name = "Administration" },
                new Team { Name = "Formation" },
                new Team { Name = "Maintenance" },
                new Team { Name = "Qualité" }
            };

            _context.Teams.AddRange(teams);
            await _context.SaveChangesAsync();
        }

        public async Task SeedUsersAsync()
        {
            if (await _context.Users.AnyAsync()) return;

            var teams = await _context.Teams.ToListAsync();
            var random = new Random();

            var users = new List<User>
            {
                // Admin users
                new User
                {
                    UserName = "admin",
                    Password = _authService.HashPassword("admin"),
                    Email = "admin@clinisys.com",
                    Role = "Administrator",
                    FirstName = "Jean",
                    LastName = "Dupont",
                    Phone = "+33 1 23 45 67 89",
                    Address = "123 Rue de la Paix\n75001 Paris, France",
                    DateOfBirth = new DateTime(1980, 6, 15),
                    Nationality = "Française",
                    Position = "Administrateur Système",
                    Department = "IT",
                    Manager = "Direction",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Administration")?.Id,
                    LastLogin = DateTime.Now.AddHours(-2)
                },
                new User
                {
                    UserName = "marie.martin",
                    Password = _authService.HashPassword("password123"),
                    Email = "marie.martin@clinisys.com",
                    Role = "Manager",
                    FirstName = "Marie",
                    LastName = "Martin",
                    Phone = "+33 1 23 45 67 90",
                    Address = "456 Avenue des Champs\n75008 Paris, France",
                    DateOfBirth = new DateTime(1975, 3, 22),
                    Nationality = "Française",
                    Position = "Chef de Projet",
                    Department = "IT",
                    Manager = "Jean Dupont",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Développement")?.Id,
                    LastLogin = DateTime.Now.AddHours(-1)
                },
                // Technical users
                new User
                {
                    UserName = "pierre.durand",
                    Password = _authService.HashPassword("password123"),
                    Email = "pierre.durand@clinisys.com",
                    Role = "Technician",
                    FirstName = "Pierre",
                    LastName = "Durand",
                    Phone = "+33 1 23 45 67 91",
                    Address = "789 Boulevard Saint-Germain\n75007 Paris, France",
                    DateOfBirth = new DateTime(1985, 9, 10),
                    Nationality = "Française",
                    Position = "Développeur Senior",
                    Department = "IT",
                    Manager = "Marie Martin",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Développement")?.Id,
                    LastLogin = DateTime.Now.AddMinutes(-30)
                },
                new User
                {
                    UserName = "sophie.bernard",
                    Password = _authService.HashPassword("password123"),
                    Email = "sophie.bernard@clinisys.com",
                    Role = "Technician",
                    FirstName = "Sophie",
                    LastName = "Bernard",
                    Phone = "+33 1 23 45 67 92",
                    Address = "321 Rue de Rivoli\n75001 Paris, France",
                    DateOfBirth = new DateTime(1990, 12, 5),
                    Nationality = "Française",
                    Position = "Analyste Support",
                    Department = "Support",
                    Manager = "Marie Martin",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Support Technique")?.Id,
                    LastLogin = DateTime.Now.AddMinutes(-15)
                },
                new User
                {
                    UserName = "lucas.petit",
                    Password = _authService.HashPassword("password123"),
                    Email = "lucas.petit@clinisys.com",
                    Role = "User",
                    FirstName = "Lucas",
                    LastName = "Petit",
                    Phone = "+33 1 23 45 67 93",
                    Address = "654 Avenue Montaigne\n75008 Paris, France",
                    DateOfBirth = new DateTime(1988, 7, 18),
                    Nationality = "Française",
                    Position = "Technicien Infrastructure",
                    Department = "IT",
                    Manager = "Pierre Durand",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Infrastructure")?.Id,
                    LastLogin = DateTime.Now.AddHours(-3)
                },
                new User
                {
                    UserName = "emma.rousseau",
                    Password = _authService.HashPassword("password123"),
                    Email = "emma.rousseau@clinisys.com",
                    Role = "User",
                    FirstName = "Emma",
                    LastName = "Rousseau",
                    Phone = "+33 1 23 45 67 94",
                    Address = "987 Rue du Faubourg\n75011 Paris, France",
                    DateOfBirth = new DateTime(1992, 4, 25),
                    Nationality = "Française",
                    Position = "Spécialiste Sécurité",
                    Department = "Sécurité",
                    Manager = "Marie Martin",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Sécurité")?.Id,
                    LastLogin = DateTime.Now.AddDays(-1)
                },
                new User
                {
                    UserName = "thomas.moreau",
                    Password = _authService.HashPassword("password123"),
                    Email = "thomas.moreau@clinisys.com",
                    Role = "User",
                    FirstName = "Thomas",
                    LastName = "Moreau",
                    Phone = "+33 1 23 45 67 95",
                    Address = "147 Boulevard Haussmann\n75008 Paris, France",
                    DateOfBirth = new DateTime(1987, 11, 8),
                    Nationality = "Française",
                    Position = "Formateur",
                    Department = "Formation",
                    Manager = "Jean Dupont",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Formation")?.Id,
                    LastLogin = DateTime.Now.AddHours(-6)
                },
                new User
                {
                    UserName = "chloe.garcia",
                    Password = _authService.HashPassword("password123"),
                    Email = "chloe.garcia@clinisys.com",
                    Role = "User",
                    FirstName = "Chloé",
                    LastName = "Garcia",
                    Phone = "+33 1 23 45 67 96",
                    Address = "258 Rue de la République\n69002 Lyon, France",
                    DateOfBirth = new DateTime(1991, 1, 14),
                    Nationality = "Française",
                    Position = "Technicienne Maintenance",
                    Department = "Maintenance",
                    Manager = "Pierre Durand",
                    IsActive = true,
                    TeamId = teams.FirstOrDefault(t => t.Name == "Maintenance")?.Id,
                    LastLogin = DateTime.Now.AddDays(-2)
                }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }

        public async Task SeedClientsAsync()
        {
            if (await _context.Clients.AnyAsync()) return;

            var clients = new List<Client>
            {
                new Client { Name = "Hôpital Saint-Louis", Email = "contact@hopital-saint-louis.fr" },
                new Client { Name = "Clinique des Lilas", Email = "admin@clinique-lilas.fr" },
                new Client { Name = "Centre Médical Pasteur", Email = "info@centre-pasteur.fr" },
                new Client { Name = "Polyclinique du Nord", Email = "contact@polyclinique-nord.fr" },
                new Client { Name = "Hôpital Universitaire", Email = "support@hopital-universitaire.fr" },
                new Client { Name = "Clinique Sainte-Marie", Email = "admin@clinique-sainte-marie.fr" },
                new Client { Name = "Centre de Radiologie", Email = "contact@centre-radio.fr" },
                new Client { Name = "Laboratoire BioMed", Email = "info@laboratoire-biomed.fr" },
                new Client { Name = "Clinique Chirurgicale", Email = "admin@clinique-chirurgie.fr" },
                new Client { Name = "Maison de Santé", Email = "contact@maison-sante.fr" },
                new Client { Name = "Centre Oncologique", Email = "support@centre-oncologie.fr" },
                new Client { Name = "Clinique Pédiatrique", Email = "info@clinique-pediatrie.fr" }
            };

            _context.Clients.AddRange(clients);
            await _context.SaveChangesAsync();
        }

        public async Task SeedModulesAsync()
        {
            if (await _context.Modules.AnyAsync()) return;

            var modules = new List<Module>
            {
                new Module { Name = "Gestion Patients" },
                new Module { Name = "Facturation" },
                new Module { Name = "Pharmacie" },
                new Module { Name = "Laboratoire" },
                new Module { Name = "Radiologie" },
                new Module { Name = "Bloc Opératoire" },
                new Module { Name = "Urgences" },
                new Module { Name = "Hospitalisation" },
                new Module { Name = "Consultations" },
                new Module { Name = "Rendez-vous" },
                new Module { Name = "Ressources Humaines" },
                new Module { Name = "Comptabilité" },
                new Module { Name = "Stock & Inventaire" },
                new Module { Name = "Sécurité & Accès" },
                new Module { Name = "Reporting" },
                new Module { Name = "Interface HL7" },
                new Module { Name = "Sauvegarde" },
                new Module { Name = "Administration Système" }
            };

            _context.Modules.AddRange(modules);
            await _context.SaveChangesAsync();
        }

        public async Task SeedTicketsAsync()
        {
            if (await _context.Tickets.AnyAsync()) return;

            var users = await _context.Users.ToListAsync();
            var clients = await _context.Clients.ToListAsync();
            var modules = await _context.Modules.ToListAsync();
            var teams = await _context.Teams.ToListAsync();
            var random = new Random();

            var ticketTitles = new[]
            {
                "Problème de connexion au système",
                "Erreur lors de la sauvegarde des données",
                "Interface utilisateur non responsive",
                "Lenteur dans le module de facturation",
                "Bug dans le calcul des totaux",
                "Problème d'impression des rapports",
                "Erreur de synchronisation des données",
                "Dysfonctionnement du module pharmacie",
                "Problème d'accès aux dossiers patients",
                "Erreur dans l'export des données",
                "Mise à jour de sécurité requise",
                "Formation sur nouveau module",
                "Configuration serveur de sauvegarde",
                "Optimisation des performances",
                "Intégration avec système externe",
                "Maintenance préventive planifiée",
                "Résolution problème réseau",
                "Mise en place nouvelle fonctionnalité",
                "Correction bug critique",
                "Support technique urgent"
            };

            var descriptions = new[]
            {
                "Le système affiche une erreur de connexion intermittente qui empêche les utilisateurs d'accéder aux fonctionnalités principales.",
                "Les données ne se sauvegardent pas correctement, causant une perte d'informations importantes.",
                "L'interface ne s'adapte pas correctement aux différentes tailles d'écran, rendant l'utilisation difficile.",
                "Le module présente des ralentissements significatifs lors du traitement des factures.",
                "Les calculs automatiques ne fonctionnent pas correctement, générant des erreurs dans les totaux.",
                "L'impression des rapports échoue avec un message d'erreur non spécifique.",
                "La synchronisation entre les différents modules ne fonctionne plus depuis la dernière mise à jour.",
                "Le module pharmacie ne répond plus aux requêtes, bloquant la gestion des médicaments.",
                "Impossible d'accéder aux dossiers patients, erreur d'autorisation persistante.",
                "L'export des données génère des fichiers corrompus ou incomplets.",
                "Une mise à jour de sécurité critique doit être appliquée pour corriger des vulnérabilités.",
                "Formation nécessaire pour l'utilisation du nouveau module récemment installé.",
                "Configuration et test du nouveau serveur de sauvegarde pour assurer la continuité.",
                "Optimisation des requêtes base de données pour améliorer les performances globales.",
                "Intégration avec le système externe de laboratoire pour automatiser les échanges.",
                "Maintenance préventive programmée pour éviter les pannes futures.",
                "Résolution des problèmes de connectivité réseau affectant plusieurs postes.",
                "Développement et mise en place d'une nouvelle fonctionnalité demandée par les utilisateurs.",
                "Correction urgente d'un bug critique affectant la stabilité du système.",
                "Support technique d'urgence pour résoudre un problème bloquant."
            };

            var tickets = new List<Ticket>();

            for (int i = 0; i < 50; i++)
            {
                var creationDate = DateTime.Now.AddDays(-random.Next(0, 90)).AddHours(-random.Next(0, 24));
                
                tickets.Add(new Ticket
                {
                    Title = ticketTitles[random.Next(ticketTitles.Length)],
                    Description = descriptions[random.Next(descriptions.Length)],
                    CreationDate = creationDate,
                    Status = (TicketStatus)random.Next(0, 4),
                    Priority = (TicketPriority)random.Next(0, 4),
                    Type = (TicketType)random.Next(0, 3),
                    DurationDays = random.Next(1, 15),
                    DurationMonths = random.Next(0, 3),
                    RichTextComment = $"Commentaire détaillé pour le ticket #{i + 1}. Informations supplémentaires sur le contexte et les actions entreprises.",
                    ClientId = clients[random.Next(clients.Count)].Id,

                    TeamId = teams[random.Next(teams.Count)].Id,
                    ModuleId = modules[random.Next(modules.Count)].Id
                });
            }

            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllDataAsync()
        {
            // Clear in reverse order of dependencies
            _context.Tickets.RemoveRange(_context.Tickets);
            _context.Users.RemoveRange(_context.Users);
            _context.Clients.RemoveRange(_context.Clients);
            _context.Modules.RemoveRange(_context.Modules);
            _context.Teams.RemoveRange(_context.Teams);

            await _context.SaveChangesAsync();
        }
    }
}
