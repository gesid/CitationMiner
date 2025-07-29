using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class Edition
    {
        [Key]
        public int EditionId { get; set; }
        public int Year { get; set; }
        public required string Title { get; set; }
        public required string Location { get; set; }
        public DateTime Date { get; set; }
        public string? Proceedings { get; set; }
        public List<Paper>? Papers { get; set; }
    }
}
