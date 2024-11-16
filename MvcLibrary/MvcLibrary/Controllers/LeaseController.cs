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
    }
}
