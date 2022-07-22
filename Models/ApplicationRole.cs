using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;

public class ApplicationRole : IdentityRole<Guid>
{
    [MaxLength(250)]
    public string? Description { get; set; }
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = default!;
    public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = default!;
}