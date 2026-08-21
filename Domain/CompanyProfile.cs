namespace PivorkJobportal.Domain
{
    /// <summary>
    /// [Domain-Schicht] Repräsentiert das Profil eines Unternehmens im System.
    /// Enthält alle Stammdaten, Kontaktdaten und geografischen Informationen des Hauptsitzes.
    /// Liegt in der Domain, damit die Fachlogik eines Profiles unabhängig von 
    /// der UI (MVC) oder der Datenbank-Technologie wiederverwendbar bleibt.
    /// </summary>
    public class CompanyProfile
    {
        public int Id { get; set; }

        // Verbindung zum Identity-User
        public string OwnerId { get; set; } = default!;

        // Wenn eine Firma neu angelegt wird, ist das automatisch immer 'false'!
        public bool IsVerified { get; set; } = false;

        public string CompanyName { get; set; } = default!;
        public string? CompanyDescription { get; set; }

        // Kontakt & Meta
        public string? CompanyMail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? WebsiteUrl { get; set; }

        // --- ORTSANGABE 1: Hauptsitz des Unternehmens ---
        public string PostalCode { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;
        public double Latitude { get; set; } 
        public double Longitude { get; set; } 

        // Branchenlogo: später auswählbar beim Inserat erstellen
        public int? DefaultLogoId { get; set; }

        // Navigation Property: EF Core weiß dadurch, dass eine Firma viele Jobs haben kann
        public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    }
}
