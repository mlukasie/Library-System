using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Migrations;
using MvcLibrary.Models;
using MvcLibrary.ViewModels;

namespace MvcLibrary.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly MvcLibraryContext _context;

        public BookController(MvcLibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Librarian"))
            {
                var librarian_books = await _context.Book
                    .ToListAsync();

                return View("IndexLibrarian", librarian_books);
            }
            else
            {
                var oneDayAgo = DateTime.UtcNow.AddDays(-1).ToLocalTime();
                var user_books = await _context.Book
                    .Where(b => b.IsVisible) // Only include visible books
                    .Select(b => new BookViewModel
                    {
                        Id = b.Id,
                        Title = b.Title,
                        ReleaseDate = b.ReleaseDate,
                        IsReservedOrLeased = _context.Lease.Any(l => l.BookId == b.Id && l.IsActive) ||
                                                _context.Reservation.Any(r => r.BookId == b.Id && r.ReservationDate >= oneDayAgo)
                    })
    .ToListAsync();
                return View("IndexUser", user_books);
            }
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null || (!book.IsVisible && User.IsInRole("User")))
            {
                return NotFound();
            }

            return View(book);
        }
        [Authorize(Roles = ("Librarian"))]
        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Publisher,ReleaseDate,Description")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.IsVisible = true;
                book.IsAvailable = true;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Book");
            }
            return View(book);
        }

        // GET: Book/Edit/5
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Publisher,IsAvailable,IsVisible,ReleaseDate,Description")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    var existingBook = await _context.Book.FindAsync(book.Id);

                    if (existingBook != null)
                    {
                        existingBook.Title = book.Title;
                        existingBook.Author = book.Author;
                        existingBook.Publisher = book.Publisher;
                        existingBook.ReleaseDate = book.ReleaseDate;
                        existingBook.Description = book.Description;
                        existingBook.IsAvailable = book.IsAvailable;
                        existingBook.IsVisible = book.IsVisible;

                        _context.Update(existingBook);
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index");
        }

        // GET: Book/Delete/5
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Librarian"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasReservations = await _context.Reservation.AnyAsync(r => r.BookId == id);
            bool hasActiveLeases = await _context.Lease.AnyAsync(l => l.BookId == id && l.IsActive);
            bool hadLeases = await _context.Lease.AnyAsync(l => l.BookId == id && !l.IsActive);

            var book = await _context.Book.FindAsync(id);

            if (hasReservations || hasActiveLeases)
            {
                return RedirectToAction("Index");
            }
            if (hadLeases)
            {
                book.IsVisible = false;
                book.IsAvailable = false;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
