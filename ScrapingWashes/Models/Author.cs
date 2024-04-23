using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string? Instituition { get; set; }
        public string State { get; set; }
        public int PaperId { get; set; }
        public List<AuthorPaper> AuthorPapers { get; set; }
    }
}