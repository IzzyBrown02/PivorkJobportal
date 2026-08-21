using System.ComponentModel.DataAnnotations.Schema;

namespace PivorkJobportal.Domain
{
    /// <summary>
    /// [Domain-Schicht] Repräsentiert eine Stellenanzeige.
    /// Liegt in der Domain, damit die Fachlogik eines Jobangebots unabhängig von 
    /// der UI (MVC) oder der Datenbank-Technologie wiederverwendbar bleibt.
    /// </summary>
    public class JobPosting
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = default!;
        public string? JobDescription { get; set; }
        public DateTime? JobStart { get; set; }

        // --- DIE VERBINDUNG zum Unternehmen (Fremdschlüssel) ---
        public int CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; set; } = default!;

        // "MyJobs"-Ansicht des Recruiters / Multi-Recruiter-Logik
        public string OwnerId { get; set; } = string.Empty;

        // --- ORTSANGABE 2: Arbeitsort (falls Abweichungen zum Hauptsitz) ---
        public string? JobPostalCode { get; set; }
        public string? JobCity { get; set; }
        public string? JobCountry { get; set; }
        public double? JobLatitude { get; set; }
        public double? JobLongitude { get; set; }

        // --- Mehrfachauswahl für den Arbeitsort ---
        public bool IsOnSite { get; set; }
        public bool IsHomeOffice { get; set; }
        public bool IsHybrid { get; set; }     

        // --- Mehrfachauswahl für die Anstellungsart ---
        public bool IsFullTime { get; set; }
        public bool IsPartTime { get; set; } 
        public bool IsMinijob { get; set; }    
        public bool IsFreelance { get; set; }  
        public bool IsInternship { get; set; } 

        // --- Gehaltsangaben ---
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SalaryMin { get; set; } // Von-Gehalt (oder exakt)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SalaryMax { get; set; } // Bis-Gehalt (optional)
        public SalaryUnit? SalaryUnit { get; set; }

    }
}
