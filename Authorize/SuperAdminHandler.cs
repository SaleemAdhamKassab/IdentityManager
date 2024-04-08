using IdentityManager.Hellper;
using Microsoft.AspNetCore.Authorization;

namespace IdentityManager.Authorize
{
	public class SuperAdminHandler : AuthorizationHandler<SuperAdminHandler>, IAuthorizationRequirement
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminHandler requirement)
		{
			if (context.User.IsInRole(SD.Roles.SuperAdmin.ToString()))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}