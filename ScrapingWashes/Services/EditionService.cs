using ScrapingWashes.Context;
using ScrapingWashes.Models;

namespace ScrapingWashes.Services
{
    public class EditionService
    {
        private readonly AppDbContext _context;

        public EditionService(AppDbContext context)
        {
            _context = context;
        }

        public void AddEdition(Edition edition)
        {
            _context.Editions.Add(edition);
            _context.SaveChanges();
        }
    }
}
