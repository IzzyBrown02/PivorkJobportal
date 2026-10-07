using Microsoft.AspNetCore.Identity;
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
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// Initialisiert eine neue Instanz des Kontextes und gibt die Verbindungseinstellungen 
        /// (z.B. den MSSQL Connection String) an die Basisklasse von Entity Framework Core weiter.
        /// </summary>
        /// <param name="options">Die Konfigurationseinstellungen für den Kontext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Repräsentiert die Tabelle für Stellenanzeigen in der MSSQL-Datenbank.
        /// </summary>
        public DbSet<JobPosting> JobPostings { get; set; }

        /// <summary>
        /// Repräsentiert die Tabelle für Unternehmensprofile in der MSSQL-Datenbank.
        /// </summary>
        public DbSet<CompanyProfile> CompanyProfiles { get; set; }

        /// <summary>
        /// Repräsentiert die M:N-Zuordnungstabelle zwischen Recruiter-Benutzern und Unternehmensprofilen.
        /// Steuert berechtigte Benutzer, Admins innerhalb einer Firma sowie den Freigabestatus.
        /// </summary>
        public DbSet<CompanyRecruiter> CompanyRecruiters { get; set; }

        /// <summary>
        /// Konfiguriert das Datenmodell, Primärschlüssel, Indizes und Kaskadierungsverhalten (Fluent API),
        /// bevor die Datenbank-Migrationen erstellt werden.
        /// </summary>
        /// <param name="builder">Der ModelBuilder zur Definition der Entitätsbeziehungen.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Zwingend erforderlich: Initialisiert die Identity-Tabellenstrukturen
            base.OnModelCreating(builder);

            // =========================================================================
            // M:N-Relationales Mapping: CompanyRecruiter (User <-> CompanyProfile)
            // =========================================================================
            builder.Entity<CompanyRecruiter>(entity =>
            {
                // Composite Primary Key (verhindert doppelte Zuordnungen)
                entity.HasKey(cr => new { cr.UserId, cr.CompanyProfileId });

                // 1:N-Beziehung zu ApplicationUser
                entity.HasOne(cr => cr.User)
                      .WithMany()
                      .HasForeignKey(cr => cr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 1:N-Beziehung zu CompanyProfile (HIER: cp.Recruiters explizit angeben!)
                entity.HasOne(cr => cr.CompanyProfile)
                      .WithMany(cp => cp.Recruiters) // <-- Das verhindert das Schatten-Feld CompanyProfileId1
                      .HasForeignKey(cr => cr.CompanyProfileId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================================
            // 1:N-Relationales Mapping: JobPosting <-> CompanyProfile
            // =========================================================================
            builder.Entity<JobPosting>(entity =>
            {
                entity.HasOne(jp => jp.CompanyProfile)
                      .WithMany(cp => cp.JobPostings)
                      .HasForeignKey(jp => jp.CompanyProfileId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
