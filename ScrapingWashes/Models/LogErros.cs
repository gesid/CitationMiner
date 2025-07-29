using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class LogErros
    {
        [Key]
        public int LogErrosId { get; set; }
        public required string Message { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
