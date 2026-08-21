using Microsoft.EntityFrameworkCore;
using PivorkJobportal.Application;
using PivorkJobportal.Infrastructure;
using Microsoft.AspNetCore.Identity;
using PivorkJobportal.Models;
using PivorkJobportal.Infrastructure.Identity;

namespace PivorkJobportal
{
    public class Program
    {
        public static void Main(string[] args)
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
            // NEU: ASP.NET Core Identity Dienste registrieren
            // =========================================================================
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Passwort-Regeln für die Entwicklung entspannen
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

            app.Run();
        }
    }
}
