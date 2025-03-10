using WebPortal.BusinessLogic.Exceptions;
using WebPortal.Models;

namespace WebPortal.BusinessLogic.Interfaces
{
    public interface IPlanRepository
    {
        Task<Result<List<Plan>>> GetAllPlans();

        Task<Result<bool>> CreatePlan(Plan plan);

        Task<Result<bool>> UpdatePlan(Plan plan);

        Task<Result<bool>> DeletePlan(string id);
    }
}
