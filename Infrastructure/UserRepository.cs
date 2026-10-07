using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // 1. Aus der Identity-Welt in saubere Domain-Welt übersetzen
        public async Task<User?> GetByEmailAsync(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser == null) return null;

            return await MapToDomainUserAsync(identityUser);
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
                        await _userManager.AddToRoleAsync(identityUser, role.ToString());
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

        // 3. Suche anhand der ID
        public async Task<User?> GetByIdAsync(Guid id)
        {
            var identityUser = await _userManager.FindByIdAsync(id.ToString());
            if (identityUser == null) return null;

            return await MapToDomainUserAsync(identityUser);
        }

        // 4. Benutzer nach Rolle abrufen
        public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
        {
            var identityUsers = await _userManager.GetUsersInRoleAsync(role.ToString());
            var domainUsers = new List<User>();

            foreach (var identityUser in identityUsers)
            {
                var domainUser = await MapToDomainUserAsync(identityUser);
                domainUsers.Add(domainUser);
            }

            return domainUsers;
        }

        // Hilfsmethode (DRY-Prinzip): Zentrales Mapping von ApplicationUser zu Domain.User
        private async Task<User> MapToDomainUserAsync(ApplicationUser identityUser)
        {
            var roles = await _userManager.GetRolesAsync(identityUser);
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
    }
}
