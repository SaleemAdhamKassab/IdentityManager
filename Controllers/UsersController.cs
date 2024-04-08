using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityManager.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> Index()
		{
			List<UserWithRolesViewModel> usersWithRolesList = await _userManager.Users.Select(e => new UserWithRolesViewModel
			{
				ID = e.Id,
				FirstName = e.FirstName,
				LastName = e.LastName,
				Email = e.Email,
				UserName = e.UserName,
				IsLocked = e.LockoutEnd != null && e.LockoutEnd > DateTime.UtcNow ? true : false,
				Roles = _userManager.GetRolesAsync(e).Result,
				Claims = _userManager.GetClaimsAsync(e).Result
			})
			.OrderBy(e => e.FirstName).ToListAsync();

			return View(usersWithRolesList);
		}


		// Edit User
		[HttpGet]
		public async Task<IActionResult> edit(string userId)
		{
			ApplicationUser userToUpdate = await _userManager.FindByIdAsync(userId);

			if (userToUpdate is null)
				return BadRequest($"The user with ID {userId} is not exists");



			UserWithRolesViewModel model = new()
			{
				ID = userToUpdate.Id,
				FirstName = userToUpdate.FirstName,
				LastName = userToUpdate.LastName,
				Email = userToUpdate.Email,
				UserName = userToUpdate.UserName,
				Roles = await _userManager.GetRolesAsync(userToUpdate)
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> edit(UserWithRolesViewModel model)
		{
			ApplicationUser userToUpdate = await _userManager.FindByIdAsync(model.ID);

			if (userToUpdate is null)
				return BadRequest($"The user with ID {model.ID} is not exists");

			userToUpdate.FirstName = model.FirstName;
			userToUpdate.LastName = model.LastName;
			userToUpdate.Email = model.Email;

			await _userManager.UpdateAsync(userToUpdate);

			return RedirectToAction(nameof(Index));
		}

		// Manage Roles
		[HttpGet]
		public async Task<IActionResult> manageRoles(string userId)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(userId);

			if (user == null)
				return BadRequest($"The user with ID {userId} is not exists");

			var roles = await _roleManager.Roles.ToListAsync();

			List<RoleViewModel> roleViewModels = roles.Select(e => new RoleViewModel
			{
				Name = e.Name,
				IsSelected = _userManager.IsInRoleAsync(user, e.Name).Result
			}).ToList();

			UserRolesViewModel userRolesViewModel = new()
			{
				ID = user.Id,
				UserName = user.UserName,
				Roles = roleViewModels
			};

			return View(userRolesViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> manageRoles(UserRolesViewModel model)
		{
			ApplicationUser userToUpdate = await _userManager.FindByIdAsync(model.ID);

			if (userToUpdate is null)
				return BadRequest($"The user with ID {model.ID} is not exists");


			// Remove user from all roles and assign new roles to it
			await _userManager.RemoveFromRolesAsync(userToUpdate, _roleManager.Roles.Select(e => e.Name));

			foreach (RoleViewModel role in model.Roles)
			{
				if (role.IsSelected)
					await _userManager.AddToRoleAsync(userToUpdate, role.Name);
			}

			await _userManager.UpdateAsync(userToUpdate);

			return RedirectToAction(nameof(Index));
		}

		// Delete User
		[HttpGet]
		public async Task<IActionResult> delete(string userId)
		{
			ApplicationUser userToUpdate = await _userManager.FindByIdAsync(userId);

			if (userToUpdate is null)
				return BadRequest($"The user with ID {userId} is not exists");

			UserWithRolesViewModel model = new()
			{
				ID = userToUpdate.Id,
				FirstName = userToUpdate.FirstName,
				LastName = userToUpdate.LastName,
				Email = userToUpdate.Email,
				UserName = userToUpdate.UserName
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> delete(UserWithRolesViewModel model)
		{
			ApplicationUser userToDelete = await _userManager.FindByIdAsync(model.ID);

			if (userToDelete is null)
				return BadRequest($"The user with ID {model.ID} is not exists");

			await _userManager.DeleteAsync(userToDelete);

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> lockUnLock(string userId)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(userId);

			if (user is null)
				return BadRequest($"The user with ID {userId} is not exists");

			bool isUserLocked = await _userManager.IsLockedOutAsync(user);

			if (isUserLocked)
				user.LockoutEnd = DateTime.UtcNow;
			else
				user.LockoutEnd = DateTime.Now.AddMinutes(30);

			await _userManager.UpdateAsync(user);

			return RedirectToAction(nameof(Index));
		}

		// Manage User Claims
		[HttpGet]
		public async Task<IActionResult> manageUserClaims(string userId)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(userId);

			if (user is null)
				return NotFound($"The user with ID {userId} is not exists");

			var userClaims = await _userManager.GetClaimsAsync(user);

			ClaimsViewModel model = new()
			{
				User = user,
			};

			foreach (Claim claim in ClaimStore.calimsList)
			{
				ClaimSelection userClaim = new()
				{
					ClaimType = claim.Type
				};

				if (userClaims.Any(e => e.Type == claim.Type))
					userClaim.IsSelected = true;

				model.ClaimList.Add(userClaim);
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> manageUserClaims(ClaimsViewModel model)
		{
			ApplicationUser user = await _userManager.FindByIdAsync(model.User.Id);

			if (user is null)
				return NotFound($"The user with ID {model.User.Id} is not exists");

			var oldClaims = await _userManager.GetClaimsAsync(user);
			var result = await _userManager.RemoveClaimsAsync(user, oldClaims);

			if (!result.Succeeded)
				return BadRequest("Error while Removing Claims");

			result = await _userManager.AddClaimsAsync(user,
				model.ClaimList.Where(e => e.IsSelected).Select(e => new Claim(e.ClaimType, e.IsSelected.ToString())));

			if (!result.Succeeded)
				return BadRequest("Error while Adding Claims");

			return RedirectToAction(nameof(Index));
		}
	}
}