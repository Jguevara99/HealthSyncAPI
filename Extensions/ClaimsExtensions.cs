using System.Security.Claims;
using ContosoPizza.Models;
using ContosoPizza.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Extensions.Claims;

public static class ClaimsExtensions
{
    public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<ApplicationRole> roleManager, ApplicationRole role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(claim => claim.Type.Equals(ApplicationClaimTypes.Permission) && claim.Value.Equals(permission)))
            return await roleManager.AddClaimAsync(role, new Claim(ApplicationClaimTypes.Permission, permission));

        return IdentityResult.Failed();
    }

    public static async Task<IList<Claim>> GetRoleClaimsAsync(this RoleManager<ApplicationRole> roleManager, string role)
    {
        ApplicationRole appRole = await roleManager.FindByNameAsync(role);
        return (await roleManager.GetClaimsAsync(appRole));
    }
}