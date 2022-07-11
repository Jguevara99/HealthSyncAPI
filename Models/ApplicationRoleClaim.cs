using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;
public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public virtual ApplicationRole Role { get; set; } = default!;
}