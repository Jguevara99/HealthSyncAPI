using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ContosoPizza.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, 
                                                      ApplicationUserClaim, 
                                                      ApplicationUserRole, 
                                                      ApplicationUserLogin, 
                                                      ApplicationRoleClaim, 
                                                      ApplicationUserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshTokens> RefreshTokens { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", "Security");
            // Each User can have many UserClaims
            entity.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            entity.HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            entity.HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            entity.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles", "Security");
            // Each Role can have many entries in the UserRole join table
            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            entity.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });

        modelBuilder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("UserRoles", "Security");
            entity.HasKey(key => new { key.UserId, key.RoleId });
            entity.HasOne(e => e.Role)
                  .WithMany(roles => roles.UserRoles)
                  .HasForeignKey(fk => fk.RoleId)
                  .IsRequired();

            entity.HasOne(e => e.User)
                  .WithMany(user => user.UserRoles)
                  .HasForeignKey(fk => fk.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<ApplicationRoleClaim>(entity =>
        {
            entity.ToTable("RoleClaims", "Security");
            entity.HasKey(key => key.Id);
            entity.HasOne(e => e.Role)
                  .WithMany(role => role.RoleClaims)
                  .HasForeignKey(fk => fk.RoleId)
                  .IsRequired();
        });

        modelBuilder.Entity<ApplicationUserClaim>(entity =>
        {
            entity.ToTable("UserClaims", "Security");
            entity.HasKey(key => key.Id);
            entity.HasOne(e => e.User)
                  .WithMany(user => user.Claims)
                  .HasForeignKey(fk => fk.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<ApplicationUserToken>(entity =>
        {
            entity.ToTable("UserTokens", "Security");
            entity.HasKey(key => new { key.UserId, key.LoginProvider, key.Name });
            entity.HasOne(e => e.User)
                  .WithMany(user => user.Tokens)
                  .HasForeignKey(fk => fk.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<ApplicationUserLogin>(entity =>
        {
            entity.ToTable("UserLogins", "Security");
            entity.HasKey(key => new { key.LoginProvider, key.ProviderKey });
            entity.HasOne(e => e.User)
                  .WithMany(user => user.Logins)
                  .HasForeignKey(fk => fk.UserId)
                  .IsRequired();
        });

        modelBuilder.Entity<RefreshTokens>(entity =>
        {
            entity.ToTable("RefreshTokens", "Security");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(user => user.RefreshTokens)
                  .HasForeignKey(fk => fk.UserId)
                  .IsRequired();
            // entity.Property(e => e.UserId).HasConversion<string>();
        });
    }
}