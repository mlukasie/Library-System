using LibraryApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly LibraryDbContext _context;
    private readonly JwtService _jwtService;

    public UserController(LibraryDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // Rejestracja użytkownika
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User model)
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

    // Logowanie użytkownika
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        Console.WriteLine("Zapytanie dotarło do serwera.");
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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return hashedPassword == HashPassword(password);
    }
}
