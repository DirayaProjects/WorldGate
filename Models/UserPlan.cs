using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models
{
    public class UserPlan
    {
        [Key]
        public string? Id { get; set; }

        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset EndDate { get; set; }

        public Plan? Plan { get; set; }
    }
}
