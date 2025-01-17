using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Models.DTO;
using System.Security.Claims;
using System.Threading.Tasks;



[Authorize]
[ApiController]
public class ReservationController: ControllerBase
{
    private readonly LibraryDbContext _context;

    public ReservationController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet("/api/User/Reservations/")]
    [Authorize(Roles = ("User"))]
    public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetUserReservations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userReservations = await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Select(r => new ReservationDTO
            {
                Id = r.Id,
                BookId = r.Book.Id,
                UserEmail = r.User.Email,
                BookTitle = r.Book.Title,
                UserId = r.UserId,
                ReservationDate = r.ReservationDate.ToLocalTime()
            })
            .Where(r => r.UserId.ToString() == userId)
            .ToListAsync();

        return Ok(userReservations);
    }

    [HttpGet("/api/Librarian/Reservations/")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetAdminReservation()
    {
        var adminReservations = await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.User)
            .Select(r => new ReservationDTO
            {
                Id = r.Id,
                BookId = r.Book.Id,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                BookTitle = r.Book.Title,
                ReservationDate = r.ReservationDate.ToLocalTime()
            })
            .ToListAsync();
        return Ok(adminReservations);
    }

    [HttpPost("/api/ReserveBook/{id}")]
    [Authorize(Roles = ("User"))]
    public async Task<IActionResult> ReserveBook(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingReservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.BookId == id);
        var existingLease = await _context.Leases
                .FirstOrDefaultAsync(l => l.BookId == id);

        var oneDayAgo = DateTime.UtcNow.AddDays(-1);
        var book = await _context.Books.FindAsync(id);

        if (book == null || existingLease != null || (existingReservation != null && existingReservation.ReservationDate > oneDayAgo))
        {
            return Conflict(new { message = "Unable to reserve that book" });
        }

        var reservation = new Reservation
        {
            BookId = id,
            UserId = int.Parse(userId),
            ReservationDate = DateTime.UtcNow,
        };

        if (existingReservation != null && existingReservation.ReservationDate < oneDayAgo)
        {
            _context.Reservations.Remove(existingReservation);
        }

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Created();
    }

    [HttpDelete("/api/Reservation/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("/api/InactiveReservations/")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<IActionResult> DeleteInactive()
    {
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);
        var reservations = await _context.Reservations.Where(r => r.ReservationDate < oneDayAgo)
            .ToListAsync();
        foreach (var reservation in reservations)
        {
            _context.Reservations.Remove(reservation);
        }
        await _context.SaveChangesAsync();
        return NoContent();
    }




}

