using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Endpoint.Identity;

public class ApplicationUserClaim : IdentityUserClaim<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
