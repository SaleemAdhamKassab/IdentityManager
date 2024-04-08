using IdentityManager.Hellper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers
{
	[Authorize]
	public class AccessCheckerController : Controller
	{
		[AllowAnonymous]
		public IActionResult allAccess() => Ok("allAccess");

		public IActionResult authorizedAccess() => Ok("authorizedAccess");


		[Authorize(Policy = "Admin")]
		public IActionResult adminAccess() => Ok("adminAccess");


		[Authorize(Roles = "User, Admin")]
		public IActionResult userOrAdminAccess() => Ok("userOrAdminAccess");

		[Authorize(Policy = "Admin_CreateClaim")]
		public IActionResult adminAccessWithCreateClaim() => Ok("adminAccessWithCreateClaim");

		[Authorize(Policy = "Admin_CreateEditDeleteClaims")]
		public IActionResult adminAccessWithCreateEditDeleteClaims() => Ok("adminAccessWithCreateEditDeleteClaims");

		[Authorize(Policy = "userCreateEditDeleteClaimsORAdmin")]
		public IActionResult userCreateEditDeleteClaimsORAdmin() => Ok("userCreateEditDeleteClaimsORAdmin");

		[Authorize(Policy = "adminWithMoreThan1000Days")]
		public IActionResult adminWithMoreThan1000Days() => Ok("adminWithMoreThan1000Days");
	}
}