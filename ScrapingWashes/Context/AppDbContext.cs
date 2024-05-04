using Microsoft.EntityFrameworkCore;
using ScrapingWashes.Models;

namespace ScrapingWashes.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Paper> Papers { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorPaper> AuthorPapers { get; set; }
        public DbSet<LogErros> LogErros { get; set; }
    }
}
