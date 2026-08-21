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

        public IActionResult MyCompanies()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var companies = _companyRepository.GetByOwnerId(userId);
            return View(companies);
        }

        [HttpGet]
        public IActionResult Create() => View(new CompanyProfileViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CompanyProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var domainProfile = new CompanyProfile
                {
                    CompanyName = model.CompanyName,
                    CompanyMail = model.CompanyMail,
                    CompanyPhone = model.CompanyPhone,
                    WebsiteUrl = model.WebsiteUrl,
                    PostalCode = model.PostalCode,
                    City = model.City,
                    Country = model.Country,
                    OwnerId = userId,

                    // EXPLIZIT: Neue Firmen müssen erst vom Admin geprüft werden!
                    IsVerified = false
                };

                _companyRepository.Save(domainProfile);
                return RedirectToAction(nameof(MyCompanies));
            }

            return View(model);
        }
    }
}
