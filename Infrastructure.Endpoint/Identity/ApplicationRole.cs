using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Endpoint.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(250)]
    public string? Description { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = default!;
}
