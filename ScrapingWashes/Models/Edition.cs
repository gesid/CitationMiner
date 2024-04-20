using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class Edition
    {
        [Key]
        public int EditionId { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string Proceedings { get; set; }
        public List<Paper> Papers { get; set; }
    }
}
