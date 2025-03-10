using Microsoft.AspNetCore.Mvc;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.ViewModels;

namespace WebPortal.Controllers
{
    public class ChannelsController : Controller
    {

        private readonly IChannelRepository _channelRepository;

        public ChannelsController(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                var channels = await _channelRepository.GetAllChannels();

                return View(channels.Value ?? new List<Channel>());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching channels: {ex.Message}";
                return View();
            }
        }


        public async Task<IActionResult> Create()
        {
            try
            {
                var categories = await _channelRepository.GetAllCategories();
                var resolutions = await _channelRepository.GetAllResolutions();
                var sources = await _channelRepository.GetAllSources();
                var priorities = await _channelRepository.GetAllPriorities();

                var viewModel = new ChannelViewModel
                {
                    Categories = categories.Value ?? new List<Code>(),
                    Resolutions = resolutions.Value ?? new List<Code>(),
                    Sources = sources.Value ?? new List<Code>(),
                    Priorities = priorities.Value ?? new List<Code>(),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(ChannelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _channelRepository.GetAllCategories();
                var resolutions = await _channelRepository.GetAllResolutions();
                var sources = await _channelRepository.GetAllSources();
                var priorities = await _channelRepository.GetAllPriorities();

                model.Categories = categories.Value ?? new List<Code>();
                model.Resolutions = resolutions.Value ?? new List<Code>();
                model.Sources = sources.Value ?? new List<Code>();
                model.Priorities = priorities.Value ?? new List<Code>();

                return View(model);
            }

            try
            {
                var result = await _channelRepository.CreateChannel(model);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Channel created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating channel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Edit(string id)
        {

            try
            {
                var result = await _channelRepository.GetSingleChannel(id);

                Channel channel;

                if (result.IsSuccess)
                {
                    channel = result.Value;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Error;
                    return RedirectToAction("Index");
                }

                var categories = await _channelRepository.GetAllCategories();
                var resolutions = await _channelRepository.GetAllResolutions();
                var sources = await _channelRepository.GetAllSources();
                var priorities = await _channelRepository.GetAllPriorities();

                var viewModel = new ChannelViewModel
                {
                    Name = channel.Name,
                    Category = channel.Category,
                    Categories = categories.Value ?? new List<Code>(),
                    Resolutions = resolutions.Value ?? new List<Code>(),
                    Sources = sources.Value ?? new List<Code>(),
                    Priorities = priorities.Value ?? new List<Code>(),
                    Links = channel?.Links ?? new List<Link>(),
                };


                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ChannelViewModel channel)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _channelRepository.GetAllCategories();
                var resolutions = await _channelRepository.GetAllResolutions();
                var sources = await _channelRepository.GetAllSources();
                var priorities = await _channelRepository.GetAllPriorities();

                channel.Categories = categories.Value ?? new List<Code>();
                channel.Resolutions = resolutions.Value ?? new List<Code>();
                channel.Sources = sources.Value ?? new List<Code>();
                channel.Priorities = priorities.Value ?? new List<Code>();

                return View(channel);
            }


            try
            {
                var result = await _channelRepository.UpdateChannel(channel);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Channel updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(channel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating channel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _channelRepository.DeleteChannel(id);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Channel deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the channel: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}
