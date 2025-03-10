using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models
{
    public class Link
    {
        [Key]
        public string? Id { get; set; }

        [Required]
        public string? Url { get; set; }

        [Required]
        public string? Resolution { get; set; }

        [Required]
        public string? Priority { get; set; }

        [Required]
        public string? Source { get; set; }

        [Required]
        public string? Bandwidth { get; set; }

        public sbyte IsActive { get; set; } = 1;

        public sbyte IsDeleted { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
