using ContosoPizza.Extensions.Claims;
using ContosoPizza.Models;
using ContosoPizza.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Services;

public interface IDatabaseSeeder
{
    Task Initialize();
}

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public DatabaseSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Initialize()
    {
        await SeedDefaultRoles();
        await SeedSuperAdminUser();
        await SeedCustomer();
        await SeedAdministrator();
    }

    private async Task SeedSuperAdminUser()
    {
        const string password = "Gen3ricP@ssword";

        var adminUser = new ApplicationUser()
        {
            Email = "01dlopezs98@gmail.com",
            UserName = "DlopezS98",
            EmailConfirmed = true,
        };

        if ((await _userManager.FindByEmailAsync(adminUser.Email)) is not null &&
            (await _userManager.FindByNameAsync(adminUser.UserName)) is not null)
            return;

        await _userManager.CreateAsync(adminUser, password);
        await _userManager.AddToRoleAsync(adminUser, Roles.SuperAdmin);
    }

    private async Task SeedCustomer()
    {
        const string password = "Gen3ricP@ssword";
        var customer = new ApplicationUser()
        {
            Email = "customer00@gmail.com",
            UserName = "GenericUsername",
            EmailConfirmed = true
        };

        if ((await _userManager.FindByEmailAsync(customer.Email)) is not null &&
            (await _userManager.FindByNameAsync(customer.UserName)) is not null)
            return;

        await _userManager.CreateAsync(customer, password);
        await _userManager.AddToRoleAsync(customer, Roles.Customer);
    }

    private async Task SeedAdministrator()
    {
        const string password = "Gen3ricP@ssword";
        var administrator = new ApplicationUser()
        {
            Email = "admin@gmail.com",
            UserName = "AdminUsername",
            EmailConfirmed = true
        };

        if ((await _userManager.FindByEmailAsync(administrator.Email)) is not null &&
            (await _userManager.FindByNameAsync(administrator.UserName)) is not null)
            return;

        await _userManager.CreateAsync(administrator, password);
        await _userManager.AddToRoleAsync(administrator, Roles.Administrator);
    }

    private async Task SeedDefaultRoles()
    {
        var defaultRoles = new List<Tuple<string, string>>
        {
            new Tuple<string, string>(Roles.Administrator, "Administrator role with some permissions"),
            new Tuple<string, string>(Roles.Customer, "Customer permissions"),
            new Tuple<string, string>(Roles.SuperAdmin, "Administrator role with full permissions")
        };

        foreach (Tuple<string, string> defaultRole in defaultRoles)
        {
            var dbRole = await _roleManager.FindByNameAsync(defaultRole.Item1);
            if (dbRole is null)
            {
                var role = new ApplicationRole() { Name = defaultRole.Item1, Description = defaultRole.Item2 };
                await _roleManager.CreateAsync(role);

                switch (defaultRole.Item1)
                {
                    case Roles.SuperAdmin:
                        await SeedRolePermissions(role, Permissions.GetRegisteredPermissions());
                        break;
                    case Roles.Administrator:
                        await SeedRolePermissions(role, Permissions.GenerateBasePermissions4Module(typeof(Permissions.Pizzas).Name));
                        break;
                }
            }
        }
    }

    private async Task SeedRolePermissions(ApplicationRole role, List<string> permissions)
    {
        foreach (string permission in permissions)
            await _roleManager.AddPermissionClaim(role, permission);
    }
}