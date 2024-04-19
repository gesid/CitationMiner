using ScrapingWashes.Enums;

namespace ScrapingWashes.Models
{
    public class Paper
    {
        public int PaperId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Abstract { get; set; }
        public string Summary { get; set; }
        public string Keywords { get; set; }
        public ETypePaper TypePaper { get; set; }
        public string Link { get; set; }
        public string References { get; set; }
        public string Citation { get; set; }
        public List<Author> Authors { get; set; }
        public DateTime ObtenDate { get; set; } = DateTime.UtcNow;
    }
}
