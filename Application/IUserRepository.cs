using PivorkJobportal.Domain;
using System.Threading.Tasks;

namespace PivorkJobportal.Application
{
    /// <summary>
    /// [Application-Schicht] Schnittstelle (Vertrag) für den Zugriff auf Benutzerdaten.
    /// Entkoppelt die Geschäftslogik von der konkreten Identity-/Datenbank-Infrastruktur
    /// und unterstützt den zentralen Login- sowie Registrierungsablauf.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Sucht einen Benutzer asynchron anhand seiner E-Mail-Adresse, um im Login-Prozess (Seite 1.1.ab) 
        /// zu entscheiden, ob der User zum Login (Passworteingabe) oder zur Registrierung geleitet wird.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Sucht einen Benutzer asynchron anhand seiner eindeutigen ID (z. B. für Detailansichten im Admin-Bereich).
        /// </summary>
        /// <param name="id">Die eindeutige GUID des Benutzers.</param>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Holt alle Benutzer, die eine bestimmte Domänen-Rolle (z. B. Recruiter oder Jobseeker) besitzen.
        /// </summary>
        /// <param name="role">Die gesuchte Domänen-Rolle.</param>
        Task<List<User>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// Speichert einen neuen Benutzer bei der Registrierung oder aktualisiert ein bestehendes 
        /// Profil asynchron (z. B. beim Upgrade vom Jobseeker zum Recruiter).
        /// </summary>
        Task SaveAsync(User user, string password);
    }
}
