using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models
{
    public class Plan
    {

        [Key]
        public string? Id { get; set; }

        public string? PlanName { get; set; }

        public int Duration { get; set; }

        public sbyte IsDeleted { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    }
}
