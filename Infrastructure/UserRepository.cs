using Microsoft.AspNetCore.Identity;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure.Identity;
using System.Collections.Generic;
using System.Data;

namespace PivorkJobportal.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // 1. Aus der Identity-Welt in saubere Domain-Welt übersetzen
        public async Task<User?> GetByEmailAsync(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser == null) return null;

            var roles = await _userManager.GetRolesAsync(identityUser);

            // Alle Strings in Domain-Enums umwandeln und der Liste hinzufügen
            var domainRoles = new List<UserRole>();

            foreach (var roleStr in roles)
            {
                if (Enum.TryParse<UserRole>(roleStr, out var parsedRole))
                {
                    domainRoles.Add(parsedRole);
                }
            }

            return new User
            {
                Id = identityUser.Id,
                Email = identityUser.Email!,
                Firstname = identityUser.Firstname,
                Lastname = identityUser.Lastname,
                Title = identityUser.Title,
                Roles = domainRoles
            };
        }

        // 2. Aus der Domain-Welt in die Identity-Datenbank speichern
        public async Task SaveAsync(User domainUser, string password)
        {
            var identityUser = await _userManager.FindByEmailAsync(domainUser.Email);

            if (identityUser == null)
            {
                // NEUREGISTRIERUNG
                identityUser = new ApplicationUser
                {
                    Id = domainUser.Id,
                    UserName = domainUser.Email,
                    Email = domainUser.Email,
                    Title = domainUser.Title,
                    Firstname = domainUser.Firstname,
                    Lastname = domainUser.Lastname,
                    EmailConfirmed = true // Für Entwicklung auf true
                };

                // Identity kümmert sich HIER automatisch um das sichere Hashen des Passworts!
                var result = await _userManager.CreateAsync(identityUser, password);

                if (result.Succeeded)
                {
                    foreach (var role in domainUser.Roles)
                    {
                        await _userManager.AddToRoleAsync(identityUser, domainUser.Roles.ToString());
                    }
                }
            }
            else
            {
                // UPDATE: Bestehenden User aktualisieren
                identityUser.Title = domainUser.Title;
                identityUser.Firstname = domainUser.Firstname;
                identityUser.Lastname = domainUser.Lastname;


                await _userManager.UpdateAsync(identityUser);

                // Rollen aktualisieren
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);

                foreach (var role in domainUser.Roles)
                {
                    await _userManager.AddToRoleAsync(identityUser, role.ToString());
                }
            }
        }
    }
}
