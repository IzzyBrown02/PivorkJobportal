namespace PivorkJobportal.Domain
{
    public class CompanyRecruiter
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public int CompanyProfileId { get; set; }
        public CompanyProfile CompanyProfile { get; set; } = default!;

        public bool IsCompanyAdmin { get; set; } = false;
        public bool IsApprovedByCompany { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
