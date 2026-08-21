namespace PivorkJobportal.Domain
{
    /// <summary>
    /// [Domain-Schicht] Repräsentiert die zentralen Benutzerdaten.
    /// Alle Rollen teilen sich diese Basis-Klasse, um Redundanz zu vermeiden.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;

        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        /// <summary>
        /// Steuert die Rechte und die UI-Ansicht (Admin, Recruiter, Jobseeker).
        /// </summary>
        public UserRole Role { get; set; }

}
}
