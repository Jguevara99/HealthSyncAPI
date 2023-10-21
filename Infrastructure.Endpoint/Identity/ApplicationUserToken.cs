using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Endpoint.Identity;

public class ApplicationUserToken : IdentityUserToken<Guid>
{
    public virtual ApplicationUser User { get; set; } = default!;
}
