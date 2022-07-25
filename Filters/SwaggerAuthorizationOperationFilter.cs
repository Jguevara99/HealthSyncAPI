using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContosoPizza.Filters;

public class SwaggerAuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        object[] methodAttributes = context.MethodInfo.GetCustomAttributes(true);
        object[] controllerAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? new object[0];
        IEnumerable<AuthorizeAttribute> ieAuthAttributes = controllerAttributes
                                                                .Union(methodAttributes)
                                                                .OfType<AuthorizeAttribute>();

        if (!(!ieAuthAttributes.IsEmpty() && methodAttributes.OfType<AllowAnonymousAttribute>().IsEmpty())) return;

        var authAttribute = ieAuthAttributes.ToList().First();

        // Response types on secure APIs.
        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

        var securityPolicies = new List<string>();
        securityPolicies.Add($"{nameof(AuthorizeAttribute.Policy)}:{authAttribute.Policy}");
        securityPolicies.Add($"{nameof(AuthorizeAttribute.Roles)}:{authAttribute.Roles}");
        securityPolicies.Add($"{nameof(AuthorizeAttribute.AuthenticationSchemes)}:{authAttribute.AuthenticationSchemes}");

        var securityScheme = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference() { Id = "Bearer", Type = ReferenceType.SecurityScheme }
        };

        operation.Security = new List<OpenApiSecurityRequirement>() {
            new OpenApiSecurityRequirement() { { securityScheme, securityPolicies } }
        };
    }
}

public static class IEnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> ienumerable) => !(ienumerable.Count() > 0);
}