using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;
public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}