using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure;
using PivorkJobportal.Infrastructure.Identity;

namespace PivorkJobportal.Controllers
{
    [Authorize(Roles = "Admin")] // Nur für Admins zugänglich!
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobPostingRepository _jobPostingRepository;

        public AdminController(
            IUserRepository userRepository,
            ICompanyRepository companyRepository, 
            IJobPostingRepository jobPostingRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _jobPostingRepository = jobPostingRepository ?? throw new ArgumentNullException(nameof(jobPostingRepository));
        }

        // Admin-Dashboard / Startseite nach dem Login
        public IActionResult Index()
        {
            return View();
        }

        // 1. Übersicht: Noch nicht verifizierte Firmen
        public async Task<IActionResult> PendingCompanies()
        {
            var unverifiedCompanies = await _companyRepository.GetUnverifiedCompaniesAsync();
            return View(unverifiedCompanies);
        }

        // Firma freischalten (Seriösitäts-Bestätigung)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCompany(int id)
        {
            await _companyRepository.ApproveCompanyAsync(id);

            TempData["SuccessMessage"] = "Die Firma wurde erfolgreich verifiziert.";
            return RedirectToAction(nameof(PendingCompanies));
        }

        // 2. Übersicht: Jobseeker & Recruiter
        public async Task<IActionResult> Recruiters()
        {
            var recruiters = await _userRepository.GetUsersByRoleAsync(UserRole.Recruiter);
            return View();
        }

        public async Task<IActionResult> Jobseekers()
        {
            var jobseekers = await _userRepository.GetUsersByRoleAsync(UserRole.Jobseeker);
            return View();
        }

        // 3. Jobs eines Recruiters verwalten
        public async Task<IActionResult> RecruiterJobs(Guid recruiterId)
        {
            var jobs = await _jobPostingRepository.GetJobsByOwnerAsync(recruiterId);
            return View(jobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            await _jobPostingRepository.DeleteAsync(jobId);
            TempData["SuccessMessage"] = "Die Stellenanzeige wurde gelöscht.";
            return RedirectToAction(nameof(Index));
        }
    }
}
