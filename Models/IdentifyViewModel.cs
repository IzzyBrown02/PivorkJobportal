using System.ComponentModel.DataAnnotations;

namespace PivorkJobportal.Models
{
    public class IdentifyViewModel
    {
        [Required(ErrorMessage = "Bitte gib deine E-Mail-Adresse ein.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; } = string.Empty;

        // Unterscheidung, ob sich ein Bewerber oder Recruiter anmeldet
        public bool IsRecruiter { get; set; } = false;
    }
}
