using IdentityManager.Hellper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IdentityManager.Authorize
{
	public class AdminHandler : AuthorizationHandler<AdminWithMore1000DaysRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminWithMore1000DaysRequirement requirement)
		{
			if (context.User.IsInRole(SD.Roles.Admin.ToString()))
			{
				//return the ID for Logged in User
				string userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

				int numberOfAccountDays = 5000; // should replace with real method

				if (numberOfAccountDays > requirement.Days)
					context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
