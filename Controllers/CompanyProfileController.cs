using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure;
using PivorkJobportal.Models;
using System.Security.Claims;

namespace PivorkJobportal.Controllers
{
    [Authorize(Roles = nameof(UserRole.Recruiter))]
    public class CompanyProfileController : Controller
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyProfileController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<IActionResult> MyCompanies()
        {
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (string.IsNullOrEmpty(currentUserIdStr))
            {
                return Challenge();
            }

            var currentUserId = Guid.Parse(currentUserIdStr);
            var myCompanies = await _companyRepository.GetCompaniesForRecruiterAsync(currentUserId);
            return View(myCompanies);
        }

        [HttpGet]
        public IActionResult Create() => View(new CompanyProfileViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var domainProfile = new CompanyProfile
                {
                    CompanyName = model.CompanyName,
                    CompanyMail = model.CompanyMail,
                    CompanyPhone = model.CompanyPhone,
                    WebsiteUrl = model.WebsiteUrl,
                    PostalCode = model.PostalCode,
                    City = model.City,
                    Country = model.Country,
                    
                    // EXPLIZIT: Neue Firmen müssen erst vom Admin geprüft werden!
                    IsVerified = false
                };

                // Verknüpfung in der M:N-Tabelle anlegen
                domainProfile.Recruiters.Add(new CompanyRecruiter
                {
                    UserId = userId,
                    IsCompanyAdmin = true,        // Der Ersteller wird Firmen-Admin
                    IsApprovedByCompany = true,   // Eigenne Freigabe ist direkt gültig
                    JoinedAt = DateTime.UtcNow
                });

                await _companyRepository.SaveAsync(domainProfile);
                return RedirectToAction(nameof(MyCompanies));
            }

            return View(model);
        }
    }
}
