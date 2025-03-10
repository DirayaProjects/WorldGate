using WebPortal.BusinessLogic.Exceptions;
using WebPortal.Models;
using WebPortal.ViewModels;

namespace WebPortal.BusinessLogic.Interfaces
{
	public interface IUserRepository
	{
		Task<Result<List<User>>> GetAllUsers();

		Task<Result<List<Plan>>> GetAllPlans();

		Task<Result<User>> GetSingleUser(string id);

		Task<Result<bool>> CreateUser(CreateUserViewModel userViewModel);

		Task<Result<bool>> UpdateUser(EditUserViewModel user);

		Task<Result<bool>> DeleteUser(string id);
		Task<Dictionary<string, User>> GetAllUsersDict();
	}
}
