using IdentityManager.Hellper;
using IdentityManager.Models;
using IdentityManager.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mail;

namespace IdentityManager.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		private void addErrorsToModel(IdentityResult identityResult)
		{
			foreach (var error in identityResult.Errors)
				ModelState.AddModelError(string.Empty, $"Error Code: {error.Code}, Error Description: {error.Description}");

		}

		private string generateUserNameFromEmail(string email) => new MailAddress(email).User;

		///////////////////////// register
		public async Task<IActionResult> register()
		{
			if (!_roleManager.RoleExistsAsync(SD.Roles.Admin.ToString()).GetAwaiter().GetResult())
			{
				await _roleManager.CreateAsync(new IdentityRole(SD.Roles.Admin.ToString()));
				await _roleManager.CreateAsync(new IdentityRole(SD.Roles.User.ToString()));
			}

			RegisterViewModel model = new()
			{
				RoleList = _roleManager.Roles.Select(e => e.Name).Select(e => new SelectListItem
				{
					Value = e,
					Text = e
				}).ToList()
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid view model");

			ApplicationUser user = new()
			{
				FirstName = model.FirstName,
				LastName = model.LastName,
				UserName = generateUserNameFromEmail(model.Email),
				Email = model.Email,
				AddedOn = DateTime.Now
			};

			try
			{
				var createUserResult = await _userManager.CreateAsync(user, model.Password);

				if (createUserResult.Succeeded && !string.IsNullOrEmpty(model.RoleSelected))
				{
					await _userManager.AddToRoleAsync(user, model.RoleSelected);
					await _signInManager.SignInAsync(user, false);
					return RedirectToAction("Index", "Home");
				}

				addErrorsToModel(createUserResult);
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}

			return View(model);
		}

		///////////////////////// login
		public IActionResult login()
		{
			LoginViewModel model = new();
			return View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest("Invalid view model");

			try
			{
				var loginResult = await _signInManager.PasswordSignInAsync(generateUserNameFromEmail(model.Email), model.Password, model.RememberMe, lockoutOnFailure: true);

				if (loginResult.Succeeded)
					return RedirectToAction("Index", "Home");

				if (loginResult.IsLockedOut)
					return View("LockOut");

				ModelState.AddModelError(string.Empty, "Invalid login attemp.");
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}

			return View(model);
		}

		///////////////////////// lockout
		public IActionResult lockout() => View();


		///////////////////////// forgot password
		public IActionResult forgotPassword() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult forgotPassword(ForgotPasswordViewModel model)
		{
			return View(model);
		}

		/////////////////////////
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}