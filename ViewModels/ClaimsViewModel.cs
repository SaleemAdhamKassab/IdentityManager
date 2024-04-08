using IdentityManager.Models;
using System.ComponentModel.DataAnnotations;

namespace IdentityManager.ViewModels
{
	public class ClaimsViewModel
	{
		public ClaimsViewModel() => ClaimList = [];

		[Required]
		public ApplicationUser User { get; set; }

		public List<ClaimSelection> ClaimList { get; set; }
	}

	public class ClaimSelection
	{
		public string ClaimType { get; set; }
		public bool IsSelected { get; set; }
	}
}