using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Endpoint.Identity;

public class ApplicationUserLogin : IdentityUserLogin<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}