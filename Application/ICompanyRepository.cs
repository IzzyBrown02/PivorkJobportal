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
        /// Lädt ein spezifisches Unternehmensprofil anhand seiner eindeutigen Datenbank-ID asynchron.
        /// </summary>
        /// <param name="companyId">Die numerische Primärschlüssel-ID des Unternehmens.</param>
        /// <returns>
        /// Ein Task, der das gefundene <see cref="CompanyProfile"/> oder <c>null</c> enthält, 
        /// wenn kein Unternehmen mit dieser ID existiert.
        /// </returns>
        Task<CompanyProfile?> GetByIdAsync(int companyId);

        /// <summary>
        /// Ruft alle Unternehmensprofile asynchron ab, bei denen der Recruiter zugeordnet und von der Firma freigegeben (Approved) ist.
        /// Unterstützt das Multi-Recruiter-Modell.
        /// </summary>
        /// <param name="recruiterUserId">Die eindeutige Identity-User-ID (Guid) des Recruiters.</param>
        /// <returns>
        /// Ein Task, der eine Liste aller freigegebenen <see cref="CompanyProfile"/>-Objekte für den Recruiter enthält.
        /// </returns>
        Task<List<CompanyProfile>> GetCompaniesForRecruiterAsync(Guid recruiterUserId);

        /// <summary>
        /// Prüft asynchron, ob ein Recruiter für eine bestimmte Firma zugeordnet und freigegeben ist.
        /// </summary>
        /// <param name="recruiterUserId">Die Identity-User-ID (Guid) des Recruiters.</param>
        /// <param name="companyProfileId">Die Datenbank-ID des Firmenprofils.</param>
        /// <returns>Ein Task, der <c>true</c> zurückgibt, wenn der Recruiter berechtigt ist; sonst <c>false</c>.</returns>
        Task<bool> IsRecruiterForCompanyAsync(Guid recruiterUserId, int companyProfileId);

        /// <summary>
        /// Speichert ein Unternehmensprofil asynchron in der Datenbank. 
        /// Führt eine "Upsert"-Logik aus: Existiert das Profil noch nicht, wird es neu angelegt. 
        /// Existiert es bereits, werden die Änderungen aktualisiert.
        /// </summary>
        /// <param name="profile">Das zu speichernde oder zu aktualisierende <see cref="CompanyProfile"/>-Objekt.</param>
        Task SaveAsync(CompanyProfile profile);

        /// <summary>
        /// Ruft alle Unternehmensprofile asynchron ab, die noch nicht verifiziert wurden (Admin-Workflow).
        /// </summary>
        /// <returns>Ein Task, der die Liste aller unverifyierten Firmen enthält.</returns>
        Task<List<CompanyProfile>> GetUnverifiedCompaniesAsync();

        /// <summary>
        /// Schaltet eine Firma asynchron frei und genehmigt die zugehörigen Recruiter (Admin-Workflow).
        /// </summary>
        /// <param name="companyId">Die Datenbank-ID der freizuschaltenden Firma.</param>
        Task ApproveCompanyAsync(int companyId);
    }
}
