using System.ComponentModel.DataAnnotations;

namespace IdentityManager.ViewModels
{
	public class UserRolesViewModel
	{
		[Required]
		public string ID { get; set; }

		[Required]
		public string UserName { get; set; }

		public List<RoleViewModel> Roles { get; set; }
	}

	public class RoleViewModel
	{
		public string Name { get; set; }
		public bool IsSelected { get; set; }
	}
}
