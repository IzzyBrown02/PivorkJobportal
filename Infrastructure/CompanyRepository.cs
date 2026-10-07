using Microsoft.EntityFrameworkCore;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;

namespace PivorkJobportal.Infrastructure
{
    /// <summary>
    /// [Infrastructure.Schicht] Implementiert den Datenzugriff für Unternehmensprofile (<see cref="CompanyProfile"/>) 
    /// unter Verwendung von Entity Framework Core und dem <see cref="ApplicationDbContext"/>.
    /// </summary>
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context; 

        /// <summary>
        /// Initialisiert eine neue Instanz des <see cref="CompanyRepository"/> mit dem erforderlichen Datenbank-Kontext.
        /// </summary>
        /// <param name="context">Der per Dependency Injection bereitgestellte Entity Framework Datenbank-Kontext.</param>
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyProfile?> GetByIdAsync(int companyId)
        {
            return await _context.CompanyProfiles
                .Include(c => c.Recruiters)
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }

        public async Task<List<CompanyProfile>> GetCompaniesForRecruiterAsync(Guid recruiterUserId)
        {
            return await _context.CompanyRecruiters
                .Where(cr => cr.UserId == recruiterUserId && cr.IsApprovedByCompany)
                .Select(cr => cr.CompanyProfile)
                .ToListAsync();
        }

        public async Task<bool> IsRecruiterForCompanyAsync(Guid recruiterUserId, int companyProfileId)
        {
            return await _context.CompanyRecruiters
                .AnyAsync(cr => cr.UserId == recruiterUserId
                        && cr.CompanyProfileId == companyProfileId
                        && cr.IsApprovedByCompany);
        }

        public async Task SaveAsync(CompanyProfile profile)
        {
            if (profile.Id == 0)
            {
               await _context.CompanyProfiles.AddAsync(profile);
            }
            else
            {
                _context.CompanyProfiles.Update(profile);
            }
            await _context.SaveChangesAsync();
        }

        // 1. Alle noch unbestätigten Firmen abrufen
        public async Task<List<CompanyProfile>> GetUnverifiedCompaniesAsync()
        {
            return await _context.CompanyProfiles
                .Include(c => c.Recruiters)
                .Where(c => !c.IsVerified)
                .ToListAsync();
        }

        // 2. Firma verifizieren und verknüpfte Recruiter freischalten
        public async Task ApproveCompanyAsync(int companyId)
        {
            var company = await _context.CompanyProfiles
                .Include(c => c.Recruiters)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company != null)
            {
                company.IsVerified = true;

                // Alle Recruiter dieser Firma freischalten
                foreach (var recruiter in company.Recruiters)
                {
                    recruiter.IsApprovedByCompany = true;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
