using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure.Identity;

namespace PivorkJobportal.Infrastructure
{
    /// <summary>
    /// [Infrastructure-Schicht] Der Datenbankkontext der Anwendung.
    /// Erbt von IdentityDbContext, um die Tabellen für das Microsoft Identity-Sicherheitssystem 
    /// (Benutzerverwaltung, Rollen, Logins) automatisch bereitzustellen.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initialisiert eine neue Instanz des Kontextes und gibt die Verbindungseinstellungen 
        /// (z.B. den MSSQL Connection String) an die Basisklasse von Entity Framework Core weiter.
        /// </summary>
        /// <param name="options">Die Konfigurationseinstellungen für den Kontext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Repräsentiert die Tabelle für die Stellenanzeigen in der MSSQL-Datenbank.
        /// Entity Framework Core generiert daraus automatisch die Spalten basierend auf der Domain-Klasse.
        /// </summary>
        public DbSet<JobPosting> JobPostings { get; set; }

        /// <summary>
        /// Ruft die Datenbanksammlung (Tabelle) der Unternehmensprofile ab oder legt diese fest.
        /// Ermöglicht den LINQ-basierten CRUD-Zugriff auf die Tabelle "CompanyProfiles" in der MSSQL-Datenbank.
        /// </summary>
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }
    }
}
