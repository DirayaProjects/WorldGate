using WebPortal.BusinessLogic.Exceptions;
using WebPortal.Models;

namespace WebPortal.BusinessLogic.Interfaces
{
	public interface ICodeRepository
	{
		Task<Result<List<Code>>> GetAllCodes();

		Task<Result<Code>> GetSingleCode(string id);

		Task<Result<bool>> CreateCode(Code code);

		Task<Result<bool>> UpdateCode(Code code);

		Task<Result<bool>> DeleteCode(string id);
	}
}
