using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Migrations;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers
{ 
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly MvcLibraryContext _context;
        public ReservationController(MvcLibraryContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles = ("User"))]
        public async Task<IActionResult> Reserve(int? id)
        {
            // Get the current logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReservation = await _context.Reservation
                .FirstOrDefaultAsync(r => r.BookId == id && r.UserId == userId);

            if (existingReservation != null)
            {
                ModelState.AddModelError("", "You already have an active reservation for this book.");
                return RedirectToAction("Index", "Book");
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null || !book.IsAvailable)
            {
                ModelState.AddModelError("", "This book is currently not available for reservation.");
                return RedirectToAction("Index", "Book");
            }

            // Create a new reservation
            var reservation = new Reservation
            {
                BookId = id,
                UserId = userId,
                ReservationDate = DateTime.UtcNow,
            };

            // Mark the book as unavailable (Status = false)
            book.IsAvailable = false;
            _context.Book.Update(book);

            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Book");
        }
    }
}
