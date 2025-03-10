using FireSharp.Interfaces;
using FireSharp.Response;
using WebPortal.BusinessLogic.Exceptions;
using WebPortal.BusinessLogic.Interfaces;
using WebPortal.Models;
using WebPortal.Utilities;

namespace WebPortal.BusinessLogic.Repositories
{
	public class CodeRepository : ICodeRepository
	{

		private readonly IFirebaseClient _client;

		public CodeRepository()
		{
			FirebaseUtility firebaseUtility = new FirebaseUtility();
			_client = firebaseUtility.GetClient();
		}

		public async Task<Result<List<Code>>> GetAllCodes()
		{
			try
			{
				FirebaseResponse response = await _client.GetAsync("Codes/");

				if (response.Body == "null")
				{
					return Result<List<Code>>.Failure("No codes found.");
				}

				Dictionary<string, Code> codesDict = response.ResultAs<Dictionary<string, Code>>();
				List<Code> codes = codesDict
					.Where(u => u.Value.IsDeleted == 0)
					.Select(u =>
					{
						u.Value.Id = u.Key;
						return u.Value;
					}).ToList();

				if (codes == null || !codes.Any())
				{
					return Result<List<Code>>.Failure("No codes found.");
				}

				return Result<List<Code>>.Success(codes); ;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return Result<List<Code>>.Failure("An error occurred while fetching codes.");
			}
		}

		public async Task<Result<Code>> GetSingleCode(string id)
		{
			try
			{
				FirebaseResponse response = await _client.GetAsync($"Codes/{id}");
				if (response.Body == "null")
				{
					return Result<Code>.Failure("Code not found");
				}

				Code code = response.ResultAs<Code>();
				if (code.IsDeleted == 1)
				{
					return Result<Code>.Failure("Code is deleted");
				}

				return Result<Code>.Success(code);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return Result<Code>.Failure("An error occurred while fetching code.");
			}
		}

		public async Task<Result<bool>> CreateCode(Code code)
		{
			try
			{
				if (code == null)
				{
					return Result<bool>.Failure("Invalid code data.");
				}

				PushResponse response = await _client.PushAsync("Codes/", code);

				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					code.Id = response.Result.name;
					FirebaseResponse updateResponse = await _client.UpdateAsync($"Codes/{code.Id}", code);

					if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
					{
						return Result<bool>.Success(true);
					}
				}

				return Result<bool>.Failure("Failed to create code.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return Result<bool>.Failure("An error occurred while creating code.");
			}
		}

		public async Task<Result<bool>> UpdateCode(Code code)
		{
			try
			{
				if (code == null || string.IsNullOrEmpty(code.Id))
				{
					return Result<bool>.Failure("Invalid code data.");
				}

				FirebaseResponse response = await _client.GetAsync($"Codes/{code.Id}");
				if (response.Body == "null")
				{
					return Result<bool>.Failure("Code not found.");
				}

				Code existingCode = response.ResultAs<Code>();


				if (existingCode.IsDeleted == 1)
				{
					return Result<bool>.Failure("Cannot update a deleted code.");
				}

				FirebaseResponse updateResponse = await _client.UpdateAsync($"Codes/{code.Id}", code);

				if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
				{
					return Result<bool>.Success(true);
				}

				return Result<bool>.Failure("Failed to update code.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return Result<bool>.Failure("An error occurred while updating the code.");
			}
		}

		public async Task<Result<bool>> DeleteCode(string id)
		{
			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return Result<bool>.Failure("Invalid code ID.");
				}

				FirebaseResponse response = await _client.GetAsync($"Codes/{id}");
				if (response.Body == "null")
				{
					return Result<bool>.Failure("Code not found.");
				}

				Code code = response.ResultAs<Code>();

				if (code.IsDeleted == 1)
				{
					return Result<bool>.Failure("Code is already deleted.");
				}

				code.IsDeleted = 1;

				FirebaseResponse updateResponse = await _client.UpdateAsync($"Codes/{id}", code);

				if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
				{
					return Result<bool>.Success(true);
				}

				return Result<bool>.Failure("Failed to delete code.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return Result<bool>.Failure("An error occurred while deleting the code.");
			}
		}



	}
}
