using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Endpoint.Identity;

public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{
    public virtual ApplicationRole Role { get; set; } = default!;
}
