using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models
{
    public class Code
    {

        [Key]
        public string? Id { get; set; }

        public string? Description { get; set; }

        public int? Kind { get; set; }

        public sbyte IsDeleted { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
