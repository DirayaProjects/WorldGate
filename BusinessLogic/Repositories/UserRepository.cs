using FireSharp.Interfaces;
using FireSharp.Response;
using WebPortal.BusinessLogic.Exceptions;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.Utilities;
using WebPortal.ViewModels;

namespace WebPortal.BusinessLogic.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly IFirebaseClient _client;

        public UserRepository()
        {
            FirebaseUtility firebaseUtility = new FirebaseUtility();
            _client = firebaseUtility.GetClient();
        }


        public async Task<Result<List<User>>> GetAllUsers()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Users/");

                if (response.Body == "null")
                {
                    return Result<List<User>>.Failure("No users found.");
                }

                Dictionary<string, User> usersDict = response.ResultAs<Dictionary<string, User>>();
                List<User> users = usersDict
                    .Where(u => u.Value.IsDeleted == 0)
                    .Select(u =>
                    {
                        u.Value.Id = u.Key;
                        return u.Value;
                    }).ToList();

                if (users == null || !users.Any())
                {
                    return Result<List<User>>.Failure("No users found.");
                }

                return Result<List<User>>.Success(users); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<User>>.Failure("An error occurred while fetching users.");
            }
        }

        public async Task<Result<List<Plan>>> GetAllPlans()
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync("Plans/");

                if (response.Body == "null")
                {
                    return Result<List<Plan>>.Failure("No plans found.");
                }

                Dictionary<string, Plan> plansDict = response.ResultAs<Dictionary<string, Plan>>();
                List<Plan> plans = plansDict.Values.Where(p => p.IsDeleted == 0).ToList();

                if (plans == null || !plans.Any())
                {
                    return Result<List<Plan>>.Failure("No plans found.");
                }

                return Result<List<Plan>>.Success(plans); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<List<Plan>>.Failure("An error occurred while fetching plans.");
            }
        }


        public async Task<Result<User>> GetSingleUser(string id)
        {
            try
            {
                FirebaseResponse response = await _client.GetAsync($"Users/{id}");
                if (response.Body == "null")
                {
                    return Result<User>.Failure("User not found");
                }

                User user = response.ResultAs<User>();
                if (user.IsDeleted == 1)
                {
                    return Result<User>.Failure("User is deleted");
                }

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<User>.Failure("An error occurred while fetching user.");
            }
        }

        public async Task<Result<bool>> CreateUser(CreateUserViewModel userViewModel)
        {
            try
            {
                if (userViewModel == null)
                {
                    return Result<bool>.Failure("Invalid user data.");
                }

                var userPlan = new UserPlan();
                sbyte active;

                if (userViewModel.PlanId != null)
                {
                    FirebaseResponse planResponse = await _client.GetAsync($"Plans/{userViewModel.PlanId}");
                    if (planResponse.Body == "null")
                    {
                        return Result<bool>.Failure("Plan not found");
                    }

                    Plan plan = planResponse.ResultAs<Plan>();

                    if (plan.IsDeleted == 1)
                    {
                        return Result<bool>.Failure("Plan is deleted");
                    }

                    userPlan = new UserPlan
                    {
                        Id = Guid.NewGuid().ToString(),
                        Plan = plan,
                        EndDate = DateTimeOffset.Now.AddMonths(plan?.Duration ?? 0),
                    };

                    active = 1;
                }
                else
                {
                    userPlan = null;
                    active = 0;
                }


                var user = new User
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    PhoneNumber = userViewModel.PhoneNumber,
                    Username = userViewModel.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(userViewModel.Password),
                    PlanId = userViewModel.PlanId,
                    UserPlan = userPlan,
                    IsActive = active,
                };


                PushResponse response = await _client.PushAsync("Users/", user);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    user.Id = response.Result.name;
                    FirebaseResponse updateResponse = await _client.UpdateAsync($"Users/{user.Id}", user);

                    if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Result<bool>.Success(true);
                    }
                }

                return Result<bool>.Failure("Failed to create user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while creating user.");
            }
        }

        public async Task<Result<bool>> UpdateUser(EditUserViewModel user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    return Result<bool>.Failure("Invalid user data.");
                }


                FirebaseResponse existingUserResponse = await _client.GetAsync($"Users/{user.Id}");
                if (existingUserResponse.Body == "null")
                {
                    return Result<bool>.Failure("User not found");
                }

                User existingUser = existingUserResponse.ResultAs<User>();

                if (existingUser.IsDeleted == 1)
                {
                    return Result<bool>.Failure("Cannot update a deleted user.");
                }

                var planId = existingUser.PlanId;
                var userPlan = existingUser.UserPlan;
                sbyte active = existingUser.IsActive;

                if (existingUser.PlanId != user.PlanId)
                {

                    if (user.PlanId != null)
                    {
                        FirebaseResponse planResponse = await _client.GetAsync($"Plans/{user.PlanId}");
                        if (planResponse.Body == "null")
                        {
                            return Result<bool>.Failure("Plan not found");
                        }

                        Plan plan = planResponse.ResultAs<Plan>();

                        if (plan.IsDeleted == 1)
                        {
                            return Result<bool>.Failure("Plan is deleted");
                        }

                        userPlan = new UserPlan
                        {
                            Id = Guid.NewGuid().ToString(),
                            Plan = plan,
                            EndDate = DateTimeOffset.Now.AddMonths(plan?.Duration ?? 0),
                        };

                        active = 1;
                    }
                    else
                    {
                        planId = null;
                        userPlan = null;
                        active = 0;
                    }
                }

                if (!string.IsNullOrEmpty(user.NewPassword))
                {
                    if (!string.Equals(user.NewPassword, user.ConfirmPassword))
                    {
                        return Result<bool>.Failure("The new password and confirmation password do not match.");
                    }

                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.NewPassword);
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Username = user.Username;
                existingUser.PlanId = planId;
                existingUser.UserPlan = userPlan;
                existingUser.IsActive = active;

                FirebaseResponse response = await _client.UpdateAsync($"Users/{user.Id}", existingUser);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to update user.");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while updating the user.");
            }
        }

        public async Task<Result<bool>> DeleteUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Result<bool>.Failure("Invalid user ID.");
                }

                FirebaseResponse response = await _client.GetAsync($"Users/{id}");
                if (response.Body == "null")
                {
                    return Result<bool>.Failure("User not found.");
                }

                User user = response.ResultAs<User>();

                if (user.IsDeleted == 1)
                {
                    return Result<bool>.Failure("User is already deleted.");
                }

                user.IsDeleted = 1;

                FirebaseResponse updateResponse = await _client.UpdateAsync($"Users/{id}", user);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to delete user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while deleting the user.");
            }
        }

        public async Task<Dictionary<string, User>> GetAllUsersDict()
        {
            FirebaseResponse usersResponse = await _client.GetAsync("Users/");
            return usersResponse.Body != "null"
                ? usersResponse.ResultAs<Dictionary<string, User>>()
                : new Dictionary<string, User>();
        }

    }
}
