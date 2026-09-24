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
        /// Speichert einen neuen Benutzer bei der Registrierung oder aktualisiert ein bestehendes 
        /// Profil asynchron (z. B. beim Upgrade vom Jobseeker zum Recruiter).
        /// </summary>
        Task SaveAsync(User user, string password);
    }
}
