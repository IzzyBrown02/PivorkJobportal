using PivorkJobportal.Domain;

namespace PivorkJobportal.Application
{
    /// <summary>
    /// [Application-Schicht] Vertrag für die Verwaltung von Stellenanzeigen.
    /// Definiert alle Aktionen, die das System mit Jobs ausführen kann.
    /// </summary>
    public interface IJobPostingRepository
    {
        /// <summary>
        /// Sucht eine ganz bestimmte Stellenanzeige anhand ihrer eindeutigen ID.
        /// </summary>
        /// <param name="id">Die numerische Primärschlüssel-ID der Stellenanzeige.</param>
        /// <returns>
        /// Das gefundene <see cref="JobPosting"/> oder <c>null</c>, 
        /// wenn keine Stellenanzeige mit dieser ID existiert.
        /// </returns>
        JobPosting? GetById(int id);

        /// <summary>
        /// Holt alle im System registrierten Stellenanzeigen für die Startseite.
        /// </summary>
        IEnumerable<JobPosting> GetAll();

        /// <summary>
        /// Ruft alle verifizierten und öffentlich sichtbaren Stellenanzeigen ab.
        /// </summary>
        /// <remarks>
        /// Diese Methode filtert automatisch alle Inserate heraus, deren zugehöriges 
        /// Firmenprofil noch im Status "Pending" ist (<c>IsVerified == false</c>). 
        /// Muss für die öffentliche Jobbörse (Gäste und Jobseeker) verwendet werden, 
        /// um ungesichtete oder betrügerische Anzeigen zu verbergen.
        /// </remarks>
        /// <returns>Eine Liste von <see cref="JobPosting"/>-Objekten, deren Firmen aktiv freigeschaltet sind.</returns>
        IEnumerable<JobPosting> GetPublicJobs();

        /// <summary>
        /// Speichert eine neue Stellenanzeige oder aktualisiert eine bestehende.
        /// </summary>
        /// <param name="job">Das zu speichernde oder zu aktualisierende <see cref="JobPosting"/>-Objekt.</param>
        void Save(JobPosting job);
    }
}
