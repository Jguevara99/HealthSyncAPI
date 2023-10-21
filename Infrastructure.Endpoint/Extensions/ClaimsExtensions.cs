using Infrastructure.Endpoint.Identity;
using Infrastructure.Endpoint.Shared;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Endpoint.Extensions;

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
