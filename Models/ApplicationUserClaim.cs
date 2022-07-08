using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;
public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public virtual ApplicationUser User { get; set; }
}