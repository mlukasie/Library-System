using LibraryApi.Models.DTO;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryApi.Controllers
{
    public class LeaseController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LeaseController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/User/Leases/")]
        [Authorize(Roles = ("User"))]
        public async Task<ActionResult<IEnumerable<LeaseDTO>>> GetUserLeases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userLeases = await _context.Leases
                .Include(l => l.Book)
                .Include(l => l.User)
                .Select(l => new LeaseDTO
                {
                    Id = l.Id,
                    BookId = l.Book.Id,
                    UserEmail = l.User.Email,
                    BookTitle = l.Book.Title,
                    UserId = l.UserId,
                    IsActive = l.IsActive,
                    LeaseDate = l.LeaseDate.ToLocalTime()
                })
                .Where(l => l.UserId.ToString() == userId)
                 .OrderByDescending(l => l.IsActive)
                .ToListAsync();

            return Ok(userLeases);
        }

        [HttpGet("/api/Librarian/Leases/")]
        [Authorize(Roles = ("Librarian"))]
        public async Task<ActionResult<IEnumerable<LeaseDTO>>> GetAdminLeases()
        {
            var adminReservations = await _context.Leases
                 .Include(l => l.Book)
                .Include(l => l.User)
                .Select(l => new LeaseDTO
                {
                    Id = l.Id,
                    BookId = l.Book.Id,
                    UserEmail = l.User.Email,
                    BookTitle = l.Book.Title,
                    UserId = l.UserId,
                    IsActive = l.IsActive,
                    LeaseDate = l.LeaseDate.ToLocalTime()
                })
                 .OrderByDescending(l => l.IsActive)
                .ToListAsync();
            return Ok(adminReservations);
        }

        [HttpPost("/api/LeaseBook/{id}")]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> LeaseBook(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            var oneDayAgo = DateTime.UtcNow.AddDays(-1);

            if (reservation.ReservationDate < oneDayAgo) { 
                return Conflict(new { message = "Reservation date passed one day" });
            }
            else
            {
                Lease lease = new Lease
                {
                    BookId = reservation.BookId,
                    UserId = reservation.UserId,
                    IsActive = true,
                    LeaseDate = DateTime.UtcNow,
                };
                _context.Leases.Add(lease);
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
                return Ok(new { status = "Resource created" });
            }

        }


        [HttpPut("/api/Lease/ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var lease = await _context.Leases.FindAsync(id);
            lease.IsActive = false; 
            _context.Leases.Update(lease);
            await _context.SaveChangesAsync();
            return Ok(new { id = lease.Id, name = "Updated resource", status = "Success" });
        }

    }
}
