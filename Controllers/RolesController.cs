using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManager.Controllers
{
	[Authorize(Roles = "Admin")]
	public class RolesController : Controller
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		public RolesController(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;


		public IActionResult Index()
		{
			List<IdentityRole> rolesList = _roleManager.Roles.OrderBy(e => e.Name).ToList();

			return View(rolesList);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> addRole(string roleName)
		{
			if (await _roleManager.RoleExistsAsync(roleName))
				return BadRequest($"The Role {roleName} is already Exists");

			IdentityRole newIdentityRole = new()
			{
				Name = roleName,
				NormalizedName = roleName.ToUpper()
			};

			await _roleManager.CreateAsync(newIdentityRole);
			return RedirectToAction(nameof(Index));
		}


		// Edit Role
		public async Task<IActionResult> Edit(string roleName)
		{
			IdentityRole roleToUpdate = await _roleManager.FindByNameAsync(roleName);

			if (roleToUpdate is null)
				return BadRequest($"The role with name {roleName} is not exists");

			return View(roleToUpdate);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(IdentityRole identityRole)
		{
			IdentityRole roleToUpdate = await _roleManager.FindByIdAsync(identityRole.Id);

			if (roleToUpdate is null)
				return BadRequest($"The role with name {identityRole.Name} is not exists");


			roleToUpdate.Name = identityRole.Name;
			roleToUpdate.NormalizedName = identityRole.Name.ToUpper();

			await _roleManager.UpdateAsync(roleToUpdate);

			return RedirectToAction(nameof(Index));
		}

		// Delete Role
		public async Task<IActionResult> Delete(string roleName)
		{
			IdentityRole roleToDelete = await _roleManager.FindByNameAsync(roleName);

			if (roleToDelete is null)
				return BadRequest($"The role with name {roleName} is not exists");

			return View(roleToDelete);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "onlySuperAdminChecker")]
		public async Task<IActionResult> Delete(IdentityRole identityRole)
		{
			IdentityRole roleToDelete = await _roleManager.FindByIdAsync(identityRole.Id);

			if (roleToDelete is null)
				return BadRequest($"The role with name {identityRole.Name} is not exists");


			await _roleManager.DeleteAsync(roleToDelete);

			return RedirectToAction(nameof(Index));
		}
	}
}
