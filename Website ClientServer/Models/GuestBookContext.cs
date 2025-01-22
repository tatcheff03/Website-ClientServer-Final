using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class GuestBookContext:DbContext

    {
        public GuestBookContext() : base("GuestBook") { }

        public DbSet<GuestBookEntry> Entries { get; set; }
    }
}