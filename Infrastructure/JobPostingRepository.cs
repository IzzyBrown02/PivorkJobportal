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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<JobPosting?> GetByIdAsync(int id)
        {
            return await _context.JobPostings.FindAsync(id);
        }

        public async Task<List<JobPosting>> GetAllAsync()
        {
            return await _context.JobPostings.AsNoTracking().ToListAsync();
        }

        public async Task<List<JobPosting>> GetPublicJobsAsync()
        {
            return await _context.JobPostings
                .AsNoTracking()
                .Include(j => j.CompanyProfile) // Lädt die Firma zum Job dazu
                .Where(j => j.CompanyProfile != null && j.CompanyProfile.IsVerified) 
                .ToListAsync();
        }

        public async Task<List<JobPosting>> GetJobsByOwnerAsync(Guid ownerId)
        {
            return await _context.JobPostings
                .Include(j => j.CompanyProfile)
                .Where(j => j.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task SaveAsync(JobPosting job)
        {
            // schaut in der Datenbank (j) nach, ob es einen Job mit dieser ID bereits gibt
            var exists = _context.JobPostings.Any(j => j.Id == job.Id);

            if (!exists)
            {
                await _context.JobPostings.AddAsync(job);
            }
            else
            {
                _context.JobPostings.Update(job);
            }

            // schreibt die Daten live in die MSSQL-Datenbank.
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job != null)
            {
                _context.JobPostings.Remove(job);
                await _context.SaveChangesAsync();
            }
        }
    }
}
