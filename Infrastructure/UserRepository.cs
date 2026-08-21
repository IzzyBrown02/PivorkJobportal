using Microsoft.AspNetCore.Identity;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure.Identity;

namespace PivorkJobportal.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. Aus der Identity-Welt in deine saubere Domain-Welt übersetzen
        public async Task<User?> GetByEmailAsync(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser == null) return null;

            var roles = await _userManager.GetRolesAsync(identityUser);
            // Enum parsen (z.B. "Recruiter" -> UserRole.Recruiter)
            Enum.TryParse(roles.FirstOrDefault(), out UserRole domainRole);

            return new User
            {
                Id = Guid.Parse(identityUser.Id), // Identity nutzt Strings für IDs
                Email = identityUser.Email!,
                Firstname = identityUser.Firstname,
                Lastname = identityUser.Lastname,
                Title = identityUser.Title,
                Role = domainRole
                // Passwort wird hier NICHT zurückgemappt (Sicherheit!)
            };
        }

        // 2. Aus der Domain-Welt in die Identity-Datenbank speichern
        public async Task SaveAsync(User domainUser)
        {
            var identityUser = await _userManager.FindByEmailAsync(domainUser.Email);

            if (identityUser == null)
            {
                // NEUREGISTRIERUNG: IdentityUser erstellen
                identityUser = new ApplicationUser
                {
                    Id = domainUser.Id.ToString(),
                    UserName = domainUser.Email,
                    Email = domainUser.Email,
                    Title = domainUser.Title,
                    Firstname = domainUser.Firstname,
                    Lastname = domainUser.Lastname,
                    EmailConfirmed = true // Für Entwicklung auf true
                };

                // Identity kümmert sich HIER automatisch um das sichere Hashen des Passworts!
                var result = await _userManager.CreateAsync(identityUser, domainUser.Password);

                if (result.Succeeded)
                {
                    // Rolle zuweisen (z.B. "Recruiter")
                    await _userManager.AddToRoleAsync(identityUser, domainUser.Role.ToString());
                }
            }
            else
            {
                // UPDATE: Bestehenden User aktualisieren
                identityUser.Title = domainUser.Title;
                identityUser.Firstname = domainUser.Firstname;
                identityUser.Lastname = domainUser.Lastname;
                

                await _userManager.UpdateAsync(identityUser);

                // Rollen-Update falls nötig...
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
                await _userManager.AddToRoleAsync(identityUser, domainUser.Role.ToString());
            }
        }
    }
}
