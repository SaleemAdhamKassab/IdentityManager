using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityManager.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }


		[Required, DataType(DataType.Password), StringLength(100, ErrorMessage = "The Password must be at least {0} character length", MinimumLength = 6)]
		public string Password { get; set; }

		[DataType(DataType.Password), DisplayName("Confirm Password"), Compare("Password", ErrorMessage = "The Password and Confirm Password dont match")]
		public string ConfirmPassword { get; set; }


		//Roles
		public List<SelectListItem>? RoleList { get; set; }

		[Required, Display(Name = "Role")]
		public string RoleSelected { get; set; }
	}
}