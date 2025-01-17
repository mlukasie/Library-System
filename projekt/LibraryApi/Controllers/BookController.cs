using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Models.DTO;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet("LibrarianBooks")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<ActionResult<IEnumerable<LibrarianBook>>> GetLibrarianBooks()
    {
        var books = await _context.Books.ToListAsync();
        var bookDtos = books.Select(b => new LibrarianBook
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            IsVisible = b.IsVisible,
        });
        return Ok(bookDtos);
    }

    [HttpGet("UserBooks")]
    [Authorize(Roles = ("User"))]
    public async Task<ActionResult<IEnumerable<UserBook>>> GetUserBooks()
    {
        
        var books = await _context.Books
            .Where(b => b.IsVisible)
            .ToListAsync();
        
        var bookDtos = books.Select(b => new UserBook
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            isAvailable = this.IsAvailable(b.Id).Result
        });

        return Ok(bookDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDTO>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        var bookDTO = new BookDTO
        {
            Title = book.Title,
            Author = book.Author,
            ReleaseDate = book.ReleaseDate,
            Publisher = book.Publisher,
        };
        return Ok(bookDTO);
    }

    [HttpPost]
    [Authorize(Roles = ("Librarian"))]
    public async Task<ActionResult<Book>> PostBook(BookDTO book)
    {
        var newBook = new Book
        {
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            ReleaseDate = book.ReleaseDate,
            IsVisible = true,
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<IActionResult> PutBook(int id, BookDTO bookDto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        try
        {
            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Publisher = bookDto.Publisher;
            book.ReleaseDate = bookDto.ReleaseDate;
            _context.Update(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the book.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> IsAvailable(int book_id) 
    {
        var oneDayAgo = DateTime.UtcNow.AddDays(-1);
        var existingReservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.BookId == book_id);
        var existingLease = await _context.Leases
                .FirstOrDefaultAsync(l => l.BookId == book_id);
        return existingLease != null || (existingReservation != null && existingReservation.ReservationDate > oneDayAgo) ? false : true;
    }
}
