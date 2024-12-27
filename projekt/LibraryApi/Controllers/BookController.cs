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
    public async Task<ActionResult<IEnumerable<Book>>> GetLibrarianBooks()
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
    public async Task<ActionResult<IEnumerable<Book>>> GetUserBooks()
    {
        var books = await _context.Books
            .Where(b => b.IsVisible)
            .ToListAsync();
        var bookDtos = books.Select(b => new UserBook
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            isAvailable = true,
        });
        return Ok(bookDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    [Authorize(Roles = ("Librarian"))]
    public async Task<ActionResult<Book>> PostBook(CreateBook book)
    {
        var newBook = new Book
        {
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            ReleaseDate = book.ReleaseDate.ToUniversalTime(),
            IsVisible = true,
        };

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = ("Librarian"))]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (id != book.Id)
        {
            return BadRequest();
        }

        _context.Entry(book).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Books.Any(e => e.Id == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
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
}
