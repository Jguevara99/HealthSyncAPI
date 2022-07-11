
using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;
public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}