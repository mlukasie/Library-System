using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MvcLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcLibrary.Controllers
{
    public class Start : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public Start(UserManager<User> userManager)
        {
            _userManager = userManager;
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
                    // Log in the user immediately after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Start");
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
                    HttpContext.Response.Cookies.Append("AuthToken", token);
                    return RedirectToAction("Index", "Book");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
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


