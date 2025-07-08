using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GestionTicketClinisys.Models;
using GestionTicketClinisys.TempModels;
using Microsoft.EntityFrameworkCore;

namespace Ticketing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Cette méthode est appelée par le runtime pour ajouter des services au conteneur
        public void ConfigureServices(IServiceCollection services)
        {
            // Ajout des deux contextes de base de données
            services.AddDbContext<CliniSysDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Ajout des contrôleurs MVC
            services.AddControllersWithViews();
            
            // Autres services...
        }

        // Cette méthode est appelée par le runtime pour configurer le pipeline de requêtes HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Décommenter si vous utilisez l'authentification
            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                
                // Décommenter si vous utilisez Razor Pages
                // endpoints.MapRazorPages();
            });
        }
    }
}