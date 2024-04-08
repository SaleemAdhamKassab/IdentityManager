using IdentityManager.Authorize;
using IdentityManager.Data;
using IdentityManager.Hellper;
using IdentityManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IAuthorizationHandler, AdminHandler>();
builder.Services.Configure<IdentityOptions>(e => e.Lockout.MaxFailedAccessAttempts = 3);

builder.Services.AddAuthorization(e =>
{
	e.AddPolicy("Admin", policy => policy.RequireRole(SD.Roles.Admin.ToString()));

	e.AddPolicy("Admin_CreateClaim", policy => policy.RequireRole(SD.Roles.Admin.ToString()).RequireClaim("Create", "True"));

	e.AddPolicy("Admin_CreateEditDeleteClaims",
		policy => policy.RequireRole(SD.Roles.Admin.ToString())
		.RequireClaim("Create", "True")
		.RequireClaim("Edit", "True")
		.RequireClaim("Delete", "True"));

	e.AddPolicy("userCreateEditDeleteClaimsORAdmin", policy => policy.RequireAssertion(context => (
	context.User.IsInRole("User") &&
	context.User.HasClaim(e => e.Type == "Create" && e.Value == "True") &&
	context.User.HasClaim(e => e.Type == "Edit" && e.Value == "True") &&
	context.User.HasClaim(e => e.Type == "Delete" && e.Value == "True"))
	||
	context.User.IsInRole("Admin")));

	e.AddPolicy("onlySuperAdminChecker", p => p.Requirements.Add(new SuperAdminHandler()));

	e.AddPolicy("adminWithMoreThan1000Days", p => p.Requirements.Add(new AdminWithMore1000DaysRequirement()));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
