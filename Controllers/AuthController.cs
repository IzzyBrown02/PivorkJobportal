using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PivorkJobportal.Domain;
using PivorkJobportal.Infrastructure.Identity;
using PivorkJobportal.Models;


namespace PivorkJobportal.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> UserManager)
        {
            _signInManager = signInManager;
            _userManager = UserManager;
        }

        // ==========================================
        // 1. E-Mail Abfrage
        // ==========================================
        [HttpGet]
        public IActionResult Identify(bool isRecruiter = false)
        {
            var model = new IdentifyViewModel { IsRecruiter = isRecruiter };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Identify(IdentifyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // E-Mail existiert -> Weiterleitung zum Login (Passwort-Eingabe)
                return RedirectToAction("Login", new { email = model.Email, isRecruiter = model.IsRecruiter });
            }

            // E-Mail neu -> Weiterleitung zur Registrierung
            return RedirectToAction("Register", new { email = model.Email, isRecruiter = model.IsRecruiter });
        }

        // ==========================================
        // 2. REGISTRIERUNG
        // ==========================================
        [HttpGet]
        public IActionResult Register(string email, bool isRecruiter = false)
        {
            var model = new RegisterViewModel
            {
                Email = email,
                IsRecruiter = isRecruiter
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Firstname = model.Firstname,
                Lastname = model.Lastname
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Nutzt das Enum für Typ-Sicherheit und konvertiert es sauber zu String für Identity
                UserRole selectedRole = model.IsRecruiter ? UserRole.Recruiter : UserRole.Jobseeker;

                await _userManager.AddToRoleAsync(user, selectedRole.ToString());
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Rollenbasierte Weiterleitung
                if (model.IsRecruiter)
                {
                    return RedirectToAction("Index", "JobPosting");
                }

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // ==========================================
        // 3. LOGIN
        // ==========================================
        [HttpGet]
        public IActionResult Login(string email, bool isRecruiter = false)
        {
            var model = new LoginViewModel
            {
                Email = email,
                IsRecruiter = isRecruiter
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Rollenbasierte Weiterleitung
                if (user != null && await _userManager.IsInRoleAsync(user, UserRole.Recruiter.ToString()))
                {
                    return RedirectToAction("Index", "JobPosting"); // Arbeitgeber-Dashboard
                }

                return RedirectToAction("Index", "Home"); // Bewerber-Startseite
            }

            ModelState.AddModelError(string.Empty, "Ungültiges Passwort.");
            return View(model);
        }

        // ==========================================
        // 4. LOGOUT
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
