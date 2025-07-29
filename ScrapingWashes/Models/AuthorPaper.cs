using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class AuthorPaper
    {
        [Key]
        public int AuthorPaperId { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        public int PaperId { get; set; }
        public Paper? Paper { get; set; }
    }
}
