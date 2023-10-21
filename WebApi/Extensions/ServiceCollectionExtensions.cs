using Application.Endpoint.Services;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Identity;
using Infrastructure.Endpoint.Repositories;
using Infrastructure.Endpoint.Services;
using Infrastructure.Endpoint.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(swagger =>
        {
            swagger.UseCustomGeneratorSchemaIds();
            swagger.SetupSecurityDefinitions();

            // Support xml comments for API Documentation.
            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                JWT jwt = configuration.GetSection("JWT").Get<JWT>();
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.Secret)),
                    ValidAudience = jwt.ValidAudience,
                    ValidIssuer = jwt.ValidIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = (context) =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            return context.Response.WriteAsJsonAsync(new Unauthorized("The token is expired!"));
                        }

                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        return context.Response.WriteAsJsonAsync(new Unauthorized("An unhandled error has ocurred!"));
                    },
                    OnChallenge = (context) =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            return context.Response.WriteAsJsonAsync(new Unauthorized("You are not Authorized."));
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = (context) =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        return context.Response.WriteAsJsonAsync(new BaseHttpResponse("You are not authorized to access this resource.", StatusCodes.Status403Forbidden, false));
                    }
                };
            });

        // setting up default authorization policy: authentication schemas (Bearer)
        services.AddAuthorization(options =>
        {
            Type[] nestedTypes = typeof(Permissions).GetNestedTypes();
            foreach (FieldInfo field in nestedTypes
                                    .SelectMany(t =>
                                        t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                    ))
            {
                string fieldValue = field.GetValue(null)?.ToString() ?? string.Empty;
                if (!String.IsNullOrEmpty(fieldValue))
                    options.AddPolicy(fieldValue, policy => policy.RequireClaim(ApplicationClaimTypes.Permission, fieldValue).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser());
            }

            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                            .RequireAuthenticatedUser()
                                            .Build();
        });

        // Database config...
        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ),
                ServiceLifetime.Singleton
            );

        services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

        // Identity default configuration...
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;
        });

        // configuration to send custom response when the model is not valid
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = (actionContext) =>
            {
                var modelErrors = actionContext
                    .ModelState
                    .Select(model =>
                        new ModelPropertyError()
                        {
                            Key = model.Key,
                            errors = model.Value?.Errors
                                        .Select(errors => errors.ErrorMessage) ?? new List<string>()
                        });

                return new BadRequestObjectResult(
                            new BadRequest<ModelStateError>("One or more validation errors occurred.",
                                                            new ModelStateError()
                                                            {
                                                                errors = modelErrors
                                                            }));
            };
        });

        // Register custom application dependencies...
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ContosoPizza.Services.IAuthenticationService, ContosoPizza.Services.AuthenticationService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        services.Configure<AppSettings>(configuration);
        services.Configure<JWT>(configuration.GetSection("JWT"));

        return services;
    }
}
