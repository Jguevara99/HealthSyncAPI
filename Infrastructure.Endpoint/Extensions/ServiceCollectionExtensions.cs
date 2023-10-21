using Application.Endpoint.Services;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;
using Infrastructure.Endpoint.Identity;
using Infrastructure.Endpoint.Repositories;
using Infrastructure.Endpoint.Services;
using Infrastructure.Endpoint.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Endpoint.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database config...
        string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("");
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString),
                ServiceLifetime.Singleton);

        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        // Identity default configuration...
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;
        });

        // Register custom application dependencies...
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        //services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        services.Configure<AppSettings>(configuration);
        services.Configure<JWT>(configuration.GetSection("JWT"));

        return services;
    }
}
