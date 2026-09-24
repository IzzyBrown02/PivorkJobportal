using System.ComponentModel.DataAnnotations;

namespace PivorkJobportal.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vorname ist erforderlich.")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nachname ist erforderlich.")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passwort ist erforderlich.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Passwort bestätigen")]
        [Compare("Password", ErrorMessage = "Die Passwörter stimmen nicht überein.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsRecruiter { get; set; } = false;
    }
}
