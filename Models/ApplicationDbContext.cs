using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}