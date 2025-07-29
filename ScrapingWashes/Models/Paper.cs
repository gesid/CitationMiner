using ScrapingWashes.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class Paper
    {
        [Key]
        public int PaperId { get; set; }
        public required string Title { get; set; }
        public int Year { get; set; }
        public string? Abstract { get; set; }
        public string? Summary { get; set; }
        public string? Keywords { get; set; }
        public ETypePaper? Type { get; set; }
        public required string Link { get; set; }
        public string? References { get; set; }
        public string? Citation { get; set; }
        public List<AuthorPaper>? PaperAuthors { get; set; }
        public DateTime ObtenDate { get; set; } = DateTime.UtcNow;
        public int EditionId { get; set; }
    }
}
