using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
}