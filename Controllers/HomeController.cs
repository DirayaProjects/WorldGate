using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.ViewModels;

namespace WebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ICodeRepository _codeRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChannelRepository _channelRepository;

        public HomeController(ILogger<HomeController> logger, ICodeRepository codeRepository, IPlanRepository planRepository, IUserRepository userRepository, IChannelRepository channelRepository)
        {
            _logger = logger;
            _codeRepository = codeRepository;
            _planRepository = planRepository;
            _userRepository = userRepository;
            _channelRepository = channelRepository;
        }

        public async Task<IActionResult> Index()
        {
            //Channels
            var channels = await _channelRepository.GetAllChannels();

            //Users
            var users = await _userRepository.GetAllUsers();

            //Plans
            var plans = await _planRepository.GetAllPlans();

            //Codes
            var codes = await _codeRepository.GetAllCodes();


            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                Channels = channels.Value ?? new List<Channel>(),
                Users = users.Value ?? new List<User>(),
                Plans = plans.Value ?? new List<Plan>(),
                Codes = codes.Value ?? new List<Code>(),
            };

            return View(dashboardViewModel ?? new DashboardViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
