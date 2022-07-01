using ContosoPizza.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ContosoPizza.Extensions.Swagger;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ContosoPizza.Extensions.ServiceCollection;

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

        services.Configure<AppSettings>(configuration);
        services.Configure<JWT>(configuration.GetSection("JWT"));

        return services;
    }
}