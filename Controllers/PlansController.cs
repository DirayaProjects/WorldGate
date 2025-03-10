using Microsoft.AspNetCore.Mvc;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;

namespace WebPortal.Controllers
{
    public class PlansController : Controller
    {

        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                var plans = await _planRepository.GetAllPlans();

                return View(plans.Value ?? new List<Plan>());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching plans: {ex.Message}";
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(Plan plan)
        {
            try
            {
                var result = await _planRepository.CreatePlan(plan);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Plan created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating plan: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Plan plan)
        {
            try
            {
                var result = await _planRepository.UpdatePlan(plan);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Plan updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(plan);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating plan: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _planRepository.DeletePlan(id);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Plan deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the plan: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
