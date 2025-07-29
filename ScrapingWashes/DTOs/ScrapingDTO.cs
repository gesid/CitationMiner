namespace ScrapingWashes.DTOs
{
    public class ScrapingDTO
    {
        public required string Title { get; set; }
        public required string Link { get; set; }
        public string? Date { get; set; }
        public int Year { get; set; }
        public int EditionId { get; set; }
    }
}
