using System.ComponentModel.DataAnnotations;

namespace ScrapingWashes.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string UserName { get; set; }

        [Required]
        [MaxLength(64)]
        public required string Password { get; set; }
    }
}
