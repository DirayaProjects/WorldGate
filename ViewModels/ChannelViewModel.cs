using System.ComponentModel.DataAnnotations;
using WebPortal.Models;

namespace WebPortal.ViewModels
{
    public class ChannelViewModel
    {
        [Key]
        [Display(Name = "Channel ID")]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Category { get; set; }

        public IFormFile? Icon { get; set; }

        public List<Code> Categories { get; set; } = new List<Code>();
        public List<Code> Resolutions { get; set; } = new List<Code>();
        public List<Code> Sources { get; set; } = new List<Code>();
        public List<Code> Priorities { get; set; } = new List<Code>();
        public List<Link> Links { get; set; } = new List<Link>();
    }
}
