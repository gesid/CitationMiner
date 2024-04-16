namespace ScrapingWashes.Models
{
    public class Edition
    {
        public int EditionId { get; set; }
        public int WASHESEdition { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public List<Paper> Papers { get; set; }
    }
}
