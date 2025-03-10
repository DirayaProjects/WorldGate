using System.ComponentModel.DataAnnotations;

namespace WebPortal.Models
{
    public class Channel
    {

        [Key]
        [Display(Name = "Channel ID")]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Category { get; set; }

        public string? Icon { get; set; } = "https://firebasestorage.googleapis.com/v0/b/worldgate-43164.firebasestorage.app/o/channel-icons%2Ficons8-channel-64.png?alt=media";

        public List<Link>? Links { get; set; } = new List<Link>();

        public sbyte IsDeleted { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;


    }
}
