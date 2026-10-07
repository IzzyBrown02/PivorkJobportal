using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PivorkJobportal.Application;
using PivorkJobportal.Domain;
using PivorkJobportal.Models;
using System.Security.Claims; // UNBEDINGT für die User-ID!
using System.Linq;
using PivorkJobportal.Infrastructure;

namespace PivorkJobportal.Controllers
{
    /// <summary>
    /// Presentation Layer (Controller) für die Verwaltung von Stellenanzeigen.
    /// Fungiert als Bindeglied zwischen der Benutzeroberfläche (Views) und der Anwendungslogik (Application Layer).
    /// </summary>
    public class JobPostingController : Controller
    {
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly ICompanyRepository _companyRepository;

        /// <summary>
        /// Initialisiert den Controller für die Stellenanzeigen.
        /// </summary>
        /// <param name="jobRepository">Der Verwalter für den Datenbankzugriff auf Jobanzeigen.</param>
        /// <exception cref="ArgumentNullException">Wird geworfen, wenn das jobRepository null ist, da der Controller ohne diesen Dienst nicht funktionieren kann.</exception>
        public JobPostingController(IJobPostingRepository jobRepository, ICompanyRepository companyRepository)
        {
            ArgumentNullException.ThrowIfNull(jobRepository);
            ArgumentNullException.ThrowIfNull(companyRepository);

            _jobPostingRepository = jobRepository;
            _companyRepository = companyRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Nur die verifizierten Jobs !
            var publicJobs = await _jobPostingRepository.GetPublicJobsAsync();
            return View(publicJobs);
        }

        // 1. GET: Zeigt dem Benutzer das leere Formular mit dem Firmen-Dropdown an
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Identity-Trick: schneller die ID des aktuell eingeloggten Recruiters holen
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserIdStr))
            {
                return Challenge(); // Schickt den User zum Login, falls er nicht eingeloggt ist
            }

            var currentUserId = Guid.Parse(currentUserIdStr);

            // Firmen abrufen, bei denen der Recruiter zugeordnet und freigegeben ist
            var userCompanies = await _companyRepository.GetCompaniesForRecruiterAsync(currentUserId);

            // Wir bauen das leere ViewModel und befüllen die Dropdown-Liste
            var model = new CreateJobPostingViewModel
            {
                AvailableCompanies = userCompanies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CompanyName
                }).ToList()
            };

            return View(model);
        }

        // 2. POST: Nimmt die Daten aus dem Formular entgegen und speichert sie
        [HttpPost]
        [ValidateAntiForgeryToken] // Schützt die App vor Cross-Site-Request-Forgery-Angriffen (Hacker-Schutz)
        public async Task<IActionResult> Create(CreateJobPostingViewModel model)
        {
            // 1. Die ID des aktuell eingeloggten Recruiters auslesen
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserIdStr)) return Challenge();

            var currentUserId = Guid.Parse(currentUserIdStr);

            // Sicherheits-Check & Datenbeschaffung: Wir laden das ausgewählte Firmenprofil
            var companyProfile = await _companyRepository.GetByIdAsync(model.CompanyProfileId);

            // Neu: Prfün, ob die Firma existiert UND ob der Recruiter für diese Firma berechtigt ist
            bool isAuthorizedRecruiter = companyProfile != null &&
                await _companyRepository.IsRecruiterForCompanyAsync(currentUserId, companyProfile.Id);

            if (!isAuthorizedRecruiter)
            {
                ModelState.AddModelError("CompanyProfileId", "Ungültige oder nicht berechtigte Firma ausgewählt.");
            }

            // Falls das Formular Fehler hat (z.B. Pflichtfeld fehlt)
            if (ModelState.IsValid)
            {
                // 2. Das Domain-Objekt befüllen
                var newJob = new JobPosting
                {
                    OwnerId = currentUserId!,
                    CompanyProfileId = model.CompanyProfileId, // Fremdschlüssel setzen!
                    JobTitle = model.JobTitle,
                    JobDescription = model.JobDescription,
                   
                    // Anstellungsart
                    IsOnSite = model.IsOnSite,
                    IsHomeOffice = model.IsHomeOffice,
                    IsHybrid = model.IsHybrid,
                    IsFullTime = model.IsFullTime,
                    IsPartTime = model.IsPartTime,
                    IsMinijob = model.IsMinijob,
                    IsFreelance = model.IsFreelance,
                    IsInternship = model.IsInternship,

                    // Beginn-Datum wird auf jetzt übernommen oder man gibt ein Datum ein 
                    JobStart = (model.StartMonth != null && model.StartYear != null) 
                    ? new DateTime(model.StartYear.Value, model.StartMonth.Value, 1): null,

                    // Gehalt
                    SalaryMin = model.SalaryMin,
                    SalaryMax = model.SalaryMax,
                    SalaryUnit = model.SalaryUnit,

                    // DIE ORTS-LOGIK: Wenn kein separater Job-Ort eingetragen wurde, nimm den Hauptsitz!
                    JobPostalCode = string.IsNullOrWhiteSpace(model.JobPostalCode) ? companyProfile?.PostalCode : model.JobPostalCode,
                    JobCity = string.IsNullOrWhiteSpace(model.JobCity) ? companyProfile?.City : model.JobCity,
                    JobCountry = string.IsNullOrWhiteSpace(model.JobCountry) ? companyProfile?.Country : model.JobCountry
                };

                await _jobPostingRepository.SaveAsync(newJob);

                // Alles super -> Weiterleitung zur Übersicht
                return RedirectToAction("Index");
            }

            /* --------------------------------------------------------------------------------
            * RECOVERY-LOGIK: VIEWMODEL-REKONSTRUKTION BEI VALIDIERUNGSFEHLERN
            * --------------------------------------------------------------------------------
            * Wenn der ModelState ungültig ist (z.B. Validierungsfehler im Formular), 
            * bricht der reguläre Speicherprozess ab und die View wird erneut gerendert.
            * * Da HTTP zustandslos ist, gehen alle nicht-übermittelten Modeldaten verloren.
            * Um eine 'NullReferenceException' in der Razor-View zu verhindern, muss die
            * Dropdown-Auswahlliste (AvailableCompanies) zwingend frisch aus der Datenbank
            * nachgeladen und dem ViewModel wieder zugewiesen werden.
            * -------------------------------------------------------------------------------- */
            var userCompanies = await _companyRepository.GetCompaniesForRecruiterAsync(currentUserId);
            model.AvailableCompanies = userCompanies.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.CompanyName
            }).ToList();

            // WEG B: Fehler im Formular! 
            // Wir bleiben hier und zeigen die Fehler an.
            return View(model);
        }

    }
}
