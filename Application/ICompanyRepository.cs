using PivorkJobportal.Domain;

namespace PivorkJobportal.Application
{
    /// <summary>
    /// [Application-Schicht] Definiert die Datenzugriffsmethoden für die Verwaltung von Unternehmensprofilen (Company Profiles).
    /// Bildet die Schnittstelle zwischen der Anwendungslogik (Application) und der Datenbank (Infrastructure).
    /// </summary>
    public interface ICompanyRepository
    {
        /// <summary>
        /// Lädt ein spezifisches Unternehmensprofil anhand seiner eindeutigen Datenbank-ID.
        /// </summary>
        /// <param name="companyId">Die numerische Primärschlüssel-ID des Unternehmens.</param>
        /// <returns>
        /// Das gefundene <see cref="CompanyProfile"/> oder <c>null</c>, 
        /// wenn kein Unternehmen mit dieser ID existiert.
        /// </returns>
        CompanyProfile? GetById(int companyId);

        /// <summary>
        /// Ruft alle Unternehmensprofile ab, die einem bestimmten Recruiter (Besitzer) zugeordnet sind.
        /// Unterstützt das Agentur-Modell, bei dem ein Benutzer mehrere Firmen verwalten kann.
        /// </summary>
        /// <param name="ownerId">Die eindeutige ID des Identity-Users (GUID als String) des Recruiters.</param>
        /// <returns>
        /// Eine Liste von <see cref="CompanyProfile"/>-Objekten, die dem User gehören. 
        /// Wenn der User keine Unternehmen besitzt, wird eine leere Liste zurückgegeben.
        /// </returns>
        List<CompanyProfile> GetByOwnerId(string ownerId);

        /// <summary>
        /// Speichert ein Unternehmensprofil in der Datenbank. 
        /// Führt eine "Upsert"-Logik aus: Existiert das Profil noch nicht, wird es neu angelegt. 
        /// Existiert es bereits, werden die Änderungen aktualisiert.
        /// </summary>
        /// <param name="profile">Das zu speichernde oder zu aktualisierende <see cref="CompanyProfile"/>-Objekt.</param>
        void Save(CompanyProfile profile);
    }
}
