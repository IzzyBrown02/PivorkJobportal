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

        public CompanyProfile? GetById(int companyId)
        {
            return _context.CompanyProfiles.FirstOrDefault(c => c.Id == companyId);
        }

        public List<CompanyProfile> GetByOwnerId(string ownerId)
        {
            // Sucht alle Firmenprofile, bei denen die OwnerId mit dem aktuellen User übereinstimmt
            return _context.CompanyProfiles
                           .Where(c => c.OwnerId == ownerId)
                           .ToList();
        }

        public void Save(CompanyProfile profile)
        {
            var exists = _context.CompanyProfiles.Any(c => c.Id == profile.Id);
            if (!exists)
            {
                _context.CompanyProfiles.Add(profile);
            }
            else
            {
                _context.CompanyProfiles.Update(profile);
            }
            _context.SaveChanges();
        }
    }
}
