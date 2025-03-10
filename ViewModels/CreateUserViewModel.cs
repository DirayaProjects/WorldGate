using System.ComponentModel.DataAnnotations;
using WebPortal.Models;

namespace WebPortal.ViewModels
{
	public class CreateUserViewModel
	{
		[Key]
		[Display(Name = "User ID")]
		public string? Id { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string? FirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string? LastName { get; set; }

		[Required]
		[Display(Name = "Phone Number")]
		[RegularExpression(@"^[0-9]{8}$", ErrorMessage = "Phone number must contain exactly 8 digits.")]
		[Phone]
		public string? PhoneNumber { get; set; }

		[Required]
		public string? Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Required]
		public string? PlanId { get; set; }

		public List<Plan> Plans { get; set; } = new List<Plan>();

	}
}
