using FireSharp.Interfaces;
using FireSharp.Response;
using WebPortal.BusinessLogic.Exceptions;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.Utilities;

namespace WebPortal.BusinessLogic.Repositories
{
    public class PlanRepository : IPlanRepository
    {

        private readonly IFirebaseClient _client;

        public PlanRepository()
        {
            FirebaseUtility firebaseUtility = new FirebaseUtility();
            _client = firebaseUtility.GetClient();
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
                List<Plan> plans = plansDict
                    .Where(u => u.Value.IsDeleted == 0)
                    .Select(u =>
                    {
                        u.Value.Id = u.Key;
                        return u.Value;
                    }).ToList();

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


        public async Task<Result<bool>> CreatePlan(Plan plan)
        {
            try
            {
                if (plan == null)
                {
                    return Result<bool>.Failure("Invalid plan data.");
                }

                PushResponse response = await _client.PushAsync("Plans/", plan);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    plan.Id = response.Result.name;
                    FirebaseResponse updateResponse = await _client.UpdateAsync($"Plans/{plan.Id}", plan);

                    if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Result<bool>.Success(true);
                    }
                }

                return Result<bool>.Failure("Failed to create plan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while creating plan.");
            }
        }


        public async Task<Result<bool>> UpdatePlan(Plan plan)
        {
            try
            {
                if (plan == null || string.IsNullOrEmpty(plan.Id))
                {
                    return Result<bool>.Failure("Invalid plan data.");
                }

                FirebaseResponse response = await _client.GetAsync($"Plans/{plan.Id}");
                if (response.Body == "null")
                {
                    return Result<bool>.Failure("Plan not found.");
                }

                Code existingCode = response.ResultAs<Code>();


                if (existingCode.IsDeleted == 1)
                {
                    return Result<bool>.Failure("Cannot update a deleted plan.");
                }

                FirebaseResponse updateResponse = await _client.UpdateAsync($"Plans/{plan.Id}", plan);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to update plan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while updating the plan.");
            }
        }


        public async Task<Result<bool>> DeletePlan(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Result<bool>.Failure("Invalid plan ID.");
                }

                FirebaseResponse response = await _client.GetAsync($"Plans/{id}");
                if (response.Body == "null")
                {
                    return Result<bool>.Failure("Plan not found.");
                }

                Plan plan = response.ResultAs<Plan>();

                if (plan.IsDeleted == 1)
                {
                    return Result<bool>.Failure("Plan is already deleted.");
                }

                plan.IsDeleted = 1;

                FirebaseResponse updateResponse = await _client.UpdateAsync($"Plans/{id}", plan);

                if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Failed to delete plan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Result<bool>.Failure("An error occurred while deleting the plan.");
            }
        }

    }
}
