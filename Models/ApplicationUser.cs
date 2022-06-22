using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? CustomProperty { get; set; }
}