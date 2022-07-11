using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;

public class ApplicationRole : IdentityRole<Guid>
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = default!;
}