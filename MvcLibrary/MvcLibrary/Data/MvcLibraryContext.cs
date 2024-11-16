﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Models;

namespace MvcLibrary.Data
{
    public class MvcLibraryContext : IdentityDbContext<User>
    {
        public MvcLibraryContext (DbContextOptions<MvcLibraryContext> options)
            : base(options)
        {
        }

        public DbSet<MvcLibrary.Models.Book> Book { get; set; } = default!;
        public DbSet<MvcLibrary.Models.Reservation> Reservation { get; set; } = default!;
        public DbSet<MvcLibrary.Models.Lease> Lease { get; set; }
    }
}
