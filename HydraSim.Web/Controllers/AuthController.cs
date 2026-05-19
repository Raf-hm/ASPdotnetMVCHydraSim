using System.Security.Claims;
using HydraSim.DAL.Repositories;
using HydraSim.Domain.Auth;
using HydraSim.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HydraSim.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepository _users;
        private readonly PasswordHasher _hasher;

        public AuthController(UserRepository users, PasswordHasher hasher)
        {
            _users = users;
            _hasher = hasher;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _users.GetByEmailAsync(model.Email);
            if (user == null || !_hasher.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Ongeldige e-mail of wachtwoord.");
                return View(model);
            }

            await SignInAsync(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _users.GetByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("", "Dit e-mailadres is al in gebruik.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                PasswordHash = _hasher.Hash(model.Password)
            };
            await _users.AddAsync(user);

            await SignInAsync(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }

        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
