using System.ComponentModel.DataAnnotations;

namespace PivorkJobportal.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Bitte gib deinen Vornamen an.")]
        public string Firstname { get; set; } = default!;

        [Required(ErrorMessage = "Bitte gib deinen Nachnamen an.")]
        public string Lastname { get; set; } = default!;

        [Required(ErrorMessage = "E-Mail ist ein Pflichtfeld.")]
        [EmailAddress(ErrorMessage = "Ungültige E-Mail-Adresse.")]
        public string Email { get; set; } = default!;
    }
}
