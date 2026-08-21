using Microsoft.EntityFrameworkCore;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;

namespace PivorkJobportal.Infrastructure
{
    /// <summary>
    /// [Infrastructure-Schicht] Setzt den Vertrag IJobPostingRepository um.
    /// Nutzt das Entity Framework Core und den ApplicationDbContext für den echten Datenbankzugriff.
    /// </summary>
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="JobPostingRepository"/>-Klasse.
        /// </summary>
        /// <param name="context">Der per Dependency Injection bereitgestellte Datenbankkontext.</param>
        public JobPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public JobPosting? GetById(int id)
        {
            return _context.JobPostings.Find(id);
        }

        public IEnumerable<JobPosting> GetAll()
        {
            return _context.JobPostings.AsNoTracking().ToList();
        }

        public IEnumerable<JobPosting> GetPublicJobs()
        {
            return _context.JobPostings
                .AsNoTracking()
                .Include(j => j.CompanyProfile) // Lädt die Firma zum Job dazu
                .Where(j => j.CompanyProfile != null && j.CompanyProfile.IsVerified) 
                .ToList();
        }

        public void Save(JobPosting job)
        {
            // schaut in der Datenbank (j) nach, ob es einen Job mit dieser ID bereits gibt
            var exists = _context.JobPostings.Any(j => j.Id == job.Id);

            if (!exists)
            {
                _context.JobPostings.Add(job);
            }
            else
            {
                _context.JobPostings.Update(job);
            }

            // schreibt die Daten live in die MSSQL-Datenbank.
            _context.SaveChanges();
        }
    }
}
