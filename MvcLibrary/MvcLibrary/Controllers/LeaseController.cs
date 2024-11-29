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
    public class LeaseController : Controller
    {
        private readonly MvcLibraryContext _context;
        public LeaseController(MvcLibraryContext context) 
        { 
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            if (User.IsInRole("Librarian"))
            {
                List<LeaseViewModel> librarian_leases = await _context.Lease
                    .Include(l => l.Book)
                    .Include(l => l.User)
                    .Select(l => new LeaseViewModel
                    {
                        LeaseId = l.Id,
                        UserId = l.UserId,
                        BookId = l.BookId,
                        UserName = l.User.UserName,
                        Title = l.Book.Title,
                        IsActive = l.IsActive,
                        DateLease = l.LeaseDate,
                    })
                    .ToListAsync();

                    return View("IndexLibrarian", librarian_leases);
            }
            else
            {
                string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<LeaseViewModel> user_leases = await _context.Lease
                    .Include(l => l.Book)
                    .Include(l => l.User)
                    .Select(l => new LeaseViewModel
                    {
                        LeaseId = l.Id,
                        UserId = l.UserId,
                        BookId = l.BookId,
                        UserName = l.User.UserName,
                        Title = l.Book.Title,
                        IsActive = l.IsActive,
                        DateLease = l.LeaseDate,
                    })
                    .Where(l => l.UserId == UserId)
                    .ToListAsync();

                return View("IndexUser", user_leases);
            }
        }
        [HttpPost]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Lease(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (DateTime.UtcNow.ToLocalTime() - reservation.ReservationDate > TimeSpan.FromHours(24))
            {
                var book = await _context.Book.FindAsync(reservation.BookId);
                book.IsAvailable = true;
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                Lease lease = new Lease
                {
                    BookId = reservation.BookId,
                    UserId = reservation.UserId,
                    IsActive = true,
                    LeaseDate = DateTime.UtcNow.ToLocalTime(),
                };
               
                _context.Lease.Add(lease);
                _context.Reservation.Remove(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Return(int id)
        {
            var lease = await _context.Lease.FindAsync(id);
            if (!lease.IsActive)
            {
                return RedirectToAction("Index");
            }
            var book = await _context.Book.FindAsync(lease.BookId);
            lease.IsActive = false;
            book.IsAvailable = true;
            _context.Lease.Update(lease);
            _context.Book.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
