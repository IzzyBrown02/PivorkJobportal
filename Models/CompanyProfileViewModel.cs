using System.ComponentModel.DataAnnotations;

namespace PivorkJobportal.Models
{
    public class CompanyProfileViewModel
    {
        // Wird nur beim Editieren gebraucht, beim Erstellen bleibt es 0
        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte gib den offiziellen Firmennamen an.")]
        [StringLength(100, ErrorMessage = "Der Firmenname darf nicht länger als 100 Zeichen sein.")]
        public string CompanyName { get; set; } = default!;

        [EmailAddress(ErrorMessage = "Bitte gib eine gültige E-Mail-Adresse ein.")]
        public string? CompanyMail { get; set; }

        public string? CompanyPhone { get; set; }

        [Url(ErrorMessage = "Bitte gib eine gültige Webadresse ein (z.B. https://...)")]
        public string? WebsiteUrl { get; set; }

        // --- ADRESSE ---
        [Required(ErrorMessage = "Die Postleitzahl ist ein Pflichtfeld.")]
        public string PostalCode { get; set; } = default!;

        [Required(ErrorMessage = "Die Stadt ist ein Pflichtfeld.")]
        public string City { get; set; } = default!;

        [Required(ErrorMessage = "Das Land ist ein Pflichtfeld.")]
        public string Country { get; set; } = default!;

        // Hinweis: Latitude und Longitude fehlen hier absichtlich! 
        // Warum? Weil der User sie nicht eintippt, sondern später per API 
        // (z.B. OpenStreetMap/Google Maps) im Hintergrund anhand der Adresse berechnen.
    }
}
