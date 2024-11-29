using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Migrations;
using MvcLibrary.Models;
using MvcLibrary.ViewModels;

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
                .FirstOrDefaultAsync(r => r.BookId == id);

            var oneDayAgo = DateTime.UtcNow.AddDays(-1).ToLocalTime();
            var book = await _context.Book.FindAsync(id);
            if (book == null || !book.IsVisible || (existingReservation != null && existingReservation.ReservationDate > oneDayAgo))
            {
                ModelState.AddModelError("", "This book is currently not available for reservation.");
                return RedirectToAction("Index", "Book");
            }

            var reservation = new Reservation
            {
                BookId = id,
                UserId = userId,
                //ReservationDate = DateTime.UtcNow.AddDays(-2).ToLocalTime(),
                ReservationDate = DateTime.UtcNow.ToLocalTime()
            };

            
            if (existingReservation != null && existingReservation.ReservationDate < oneDayAgo)
            {
                _context.Reservation.Remove(existingReservation);
            }

            // Mark the book as unavailable (Status = false)
            book.IsAvailable = false;
            _context.Book.Update(book);

            _context.Reservation.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Librarian"))
            {
                List<ReservedBookViewModel> librarian_reservations = await _context.Reservation
                    .Include(r  => r.Book)
                    .Include(r => r.User)
                    .Select(r => new ReservedBookViewModel
                    {
                        ReservationId = r.Id,
                        DateReserved = r.ReservationDate,
                        UserId = r.UserId,
                        UserName = r.User.UserName,
                        BookId = r.BookId,
                        Title = r.Book.Title
                    }).ToListAsync();
                return View("IndexLibrarian", librarian_reservations);
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<ReservedBookViewModel> user_reservations = await _context.Reservation.Include(r => r.Book).
                     Select(r => new ReservedBookViewModel
                     {
                         ReservationId = r.Id,
                         DateReserved = r.ReservationDate,
                         UserId = r.UserId,
                         BookId = r.BookId,
                         Title = r.Book.Title
                     })
                     .Where(r=>r.UserId == userId)
                     .ToListAsync();
                return View("IndexUser", user_reservations);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            var book = await _context.Book.FindAsync(reservation.BookId);
            book.IsAvailable = true;
            _context.Book.Update(book);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> DeleteInactive()
        {
            var oneDayAgo = DateTime.UtcNow.AddDays(-1).ToLocalTime();
            var reservations = await _context.Reservation.Where(r => r.ReservationDate < oneDayAgo)
                .ToListAsync();
            foreach (var reservation in reservations)
            {
                var book = await _context.Book.FindAsync(reservation.BookId);
                book.IsAvailable = true;
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
