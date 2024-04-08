using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IdentityManager.ViewModels
{
	public class UserWithRolesViewModel
	{
		[Required]
		public string ID { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public bool IsLocked { get; set; }

		public IEnumerable<string> Roles { get; set; }

		public IEnumerable<Claim> Claims { get; set; }
	}
}