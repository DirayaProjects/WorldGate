using WebPortal.Models;

namespace WebPortal.ViewModels
{
    public class DashboardViewModel
    {
        public List<Channel> Channels { get; set; } = new List<Channel>();

        public List<User> Users { get; set; } = new List<User>();

        public List<Plan> Plans { get; set; } = new List<Plan>();

        public List<Code> Codes { get; set; } = new List<Code>();

    }
}
