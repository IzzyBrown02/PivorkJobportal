using Microsoft.AspNetCore.Identity;

namespace PivorkJobportal.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? Title { get; set; }
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
    }
}
