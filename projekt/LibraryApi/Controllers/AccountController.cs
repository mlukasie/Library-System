using LibraryApi.Models;
using LibraryApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly JwtService _jwtService;

    public AccountController(LibraryDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // Rejestracja użytkownika
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser model)
    {
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            return BadRequest("Username already exists.");
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber,
            IsLibrarian = false,
            Email = model.Email,
            Password = HashPassword(model.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser model)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null || !VerifyPassword(model.Password, user.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = _jwtService.GenerateToken(user);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,      
            Expires = DateTime.UtcNow.AddHours(1), 
            Secure = true      
        };

        HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

        return Ok(new { message = "Login successful" });
    }

    [HttpGet("role")]
    [Authorize]
    public IActionResult GetRole()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
        return Ok(new {Role = role });
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }


    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("AuthToken");
        return Ok();
    }


    private bool VerifyPassword(string password, string hashedPassword)
    {
        return hashedPassword == HashPassword(password);
    }


}

