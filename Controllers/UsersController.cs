using Microsoft.AspNetCore.Mvc;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.ViewModels;

namespace WebPortal.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IActionResult> Index()
        {
            try
            {

                var users = await _userRepository.GetAllUsers();

                return View(users.Value ?? new List<User>());

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching users: {ex.Message}";
                return View();
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var user = await _userRepository.GetSingleUser(id);

                return View(user.Value ?? new User());
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while fetching user: {ex.Message}";
                return RedirectToAction("Index");
            }
        }



        public async Task<IActionResult> Create()
        {
            try
            {

                var plans = await _userRepository.GetAllPlans();

                var model = new CreateUserViewModel
                {
                    Plans = plans.Value ?? new List<Plan>()
                };

                return View(model);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel userViewModel)
        {

            bool numberExists = await IsNumberTaken(userViewModel.Id, userViewModel.PhoneNumber);
            if (numberExists)
            {
                ModelState.AddModelError("PhoneNumber", "This phone number is already taken.");
                return View(userViewModel);
            }

            bool usernameExists = await IsUsernameTaken(userViewModel.Id, userViewModel.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(userViewModel);
            }


            if (!ModelState.IsValid)
            {
                var plans = await _userRepository.GetAllPlans();
                userViewModel.Plans = plans.Value ?? new List<Plan>();
                return View(userViewModel);
            }


            try
            {
                var result = await _userRepository.CreateUser(userViewModel);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "User created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while creating user: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Edit(string id)
        {
            try
            {

                var plans = await _userRepository.GetAllPlans();

                var result = await _userRepository.GetSingleUser(id);

                User user;

                if (result.IsSuccess)
                {
                    user = result.Value;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Error;
                    return RedirectToAction("Index");
                }

                EditUserViewModel userModel = new EditUserViewModel
                {
                    Id = id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.Username,
                    PlanId = user.PlanId,
                    Plans = plans.Value ?? new List<Plan>()
                };


                return View(userModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel user)
        {

            bool numberExists = await IsNumberTaken(user.Id, user.PhoneNumber);
            if (numberExists)
            {
                ModelState.AddModelError("PhoneNumber", "This phone number is already taken.");
                return View(user);
            }

            bool usernameExists = await IsUsernameTaken(user.Id, user.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(user);
            }


            if (!ModelState.IsValid)
            {
                var plans = await _userRepository.GetAllPlans();
                user.Plans = plans.Value ?? new List<Plan>();
                return View(user);
            }

            try
            {
                var result = await _userRepository.UpdateUser(user);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating user: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _userRepository.DeleteUser(id);
                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "User deleted successfully.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the user: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<bool> IsUsernameTaken(string? id, string? username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            try
            {
                Dictionary<string, User> usersDict = await _userRepository.GetAllUsersDict();

                return usersDict.Values.Any(u => !string.IsNullOrEmpty(u.Username) && u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Id != id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error checking username: " + ex.Message);
                return false;
            }
        }

        private async Task<bool> IsNumberTaken(string? id, string? number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            try
            {

                Dictionary<string, User> usersDict = await _userRepository.GetAllUsersDict();

                return usersDict.Values.Any(u => !string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Equals(number, StringComparison.OrdinalIgnoreCase) && u.Id != id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error checking phone number: " + ex.Message);
                return false;
            }
        }



        public bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }
    }
}
