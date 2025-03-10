using Microsoft.AspNetCore.Mvc;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;

namespace WebPortal.Controllers
{
    public class CodesController : Controller
    {

        private readonly ICodeRepository _codeRepository;

        public CodesController(ICodeRepository codeRepository)
        {
            _codeRepository = codeRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                var codes = await _codeRepository.GetAllCodes();

                return View(codes.Value ?? new List<Code>());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching codes: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(Code code)
        {
            try
            {
                var result = await _codeRepository.CreateCode(code);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Code created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating code: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Code code)
        {
            try
            {
                var result = await _codeRepository.UpdateCode(code);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Code updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(code);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating code: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _codeRepository.DeleteCode(id);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Code deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the code: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }




    }
}
