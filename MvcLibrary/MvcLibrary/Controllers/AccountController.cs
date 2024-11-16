using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MvcLibrary.Data;
using MvcLibrary.Models;
using MvcLibrary.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcLibrary.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly MvcLibraryContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, MvcLibraryContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var userView = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            return View("Details", userView);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            if(User.IsInRole("Librarian"))
            {
                return RedirectToAction("MyDetails");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Reservation> reservations = await _context.Reservation
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (reservations.Count == 0)
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.DeleteAsync(user);
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("MyDetails");
        }

        [Authorize(Roles = ("Librarian"))]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userView = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            id = null;
            return View(userView);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    IsLibrarian = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = GenerateJwtToken(user);

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,       // Makes the cookie accessible only by the server
                        Expires = DateTime.UtcNow.AddHours(1), // Sets the cookie to expire in 24 hours
                        Secure = true          // Ensures the cookie is sent over HTTPS
                    };

                    HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);
                    return RedirectToAction("Index", "Book");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login", "Account");
        }


        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("1234567890abcdef1234567890abcdef1234567890abcdef"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            claims.Add(new Claim(ClaimTypes.Role, user.IsLibrarian ? "Librarian" : "User"));

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7143",
                audience: "https://localhost:7143",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


