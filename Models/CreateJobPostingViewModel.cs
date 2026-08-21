using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PivorkJobportal.Domain; // Um an das SalaryUnit-Enum zu kommen

namespace PivorkJobportal.Models
{
    public class CreateJobPostingViewModel
    {
        // --- Echtes Pflichtfeld ---
        [Required(ErrorMessage = "Bitte gib einen aussagekräftigen Jobtitel an.")]
        [StringLength(100, ErrorMessage = "Der Jobtitel darf nicht länger als 100 Zeichen sein.")]
        public string JobTitle { get; set; } = default!;

        public string? JobDescription { get; set; }

        // --- Startdatum (Optional, da "ab sofort" möglich) ---
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }

        // --- Arbeitsort ---
        public bool IsOnSite { get; set; }
        public bool IsHomeOffice { get; set; }
        public bool IsHybrid { get; set; }

        // --- Anstellungsart ---
        public bool IsFullTime { get; set; }
        public bool IsPartTime { get; set; }
        public bool IsMinijob { get; set; }
        public bool IsFreelance { get; set; }
        public bool IsInternship { get; set; }

        // --- Gehaltsangaben (optional!) ---
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public SalaryUnit? SalaryUnit { get; set; }

        // --- Unternehmensauswahl ---
        [Required(ErrorMessage = "Bitte wähle ein Unternehmen aus, für das diese Anzeige geschaltet wird.")]
        public int CompanyProfileId { get; set; }

        public List<SelectListItem>? AvailableCompanies { get; set; }

        // --- Ortsangabe 2: Falls der Job woanders ist als der Hauptsitz der ausgewählten Unternehmen ---
        public string? JobPostalCode { get; set; }
        public string? JobCity { get; set; }
        public string? JobCountry { get; set; }

        // Hinweis: Latitude und Longitude fehlen hier absichtlich! 
        // Warum? Weil der User sie nicht eintippt, sondern später per API 
        // (z.B. OpenStreetMap/Google Maps) im Hintergrund anhand der Adresse berechnen.
    }
}
