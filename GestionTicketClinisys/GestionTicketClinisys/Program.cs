using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

using GestionTicketClinisys.Models;
using GestionTicketClinisys.TempModels;
using Microsoft.EntityFrameworkCore;

namespace Ticketing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Applique les migrations automatiquement au démarrage
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate(); // Applique les migrations pendantes
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Erreur lors de l'application des migrations");
                }
            }


            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                            logging.AddDebug();
                            logging.SetMinimumLevel(LogLevel.Debug);
                        })
                        .UseUrls("http://localhost:5000;https://localhost:5001"); // URLs explicites
                });
    }
}