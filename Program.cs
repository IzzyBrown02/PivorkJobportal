using Microsoft.EntityFrameworkCore;
using PivorkJobportal.Application;
using PivorkJobportal.Infrastructure;
using Microsoft.AspNetCore.Identity;
using PivorkJobportal.Models;
using PivorkJobportal.Infrastructure.Identity;
using PivorkJobportal.Domain;

namespace PivorkJobportal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Passwort-Versteck: holen den Connection-String sicher aus der appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Datenbank: übergeben den String an den Microsoft SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // =========================================================================
            // ASP.NET Core Identity Dienste registrieren
            // =========================================================================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                // Passwort-Regeln für die Entwicklung einfach gehalten
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            // =========================================================================

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJobPostingRepository, JobPostingRepository>();
            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Wer bist du?
            app.UseAuthentication();

            // Was darfst du?
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // =========================================================================
            // Rollen aus dem Enum(UserRole) beim Starten in der Datenbank sicherstellen
            // =========================================================================
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                // Greift direkt auf Enum zu
                var roleNames = Enum.GetNames(typeof(UserRole)); // Passe "UserRole" an den Namen des Enums an

                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                    }
                }
            }

            await app.RunAsync();
        }
    }
}
