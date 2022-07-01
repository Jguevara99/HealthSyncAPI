using System.ComponentModel;
using ContosoPizza.Filters;
using Microsoft.OpenApi.Models;
using SwaggerNamespace = Swashbuckle.AspNetCore.SwaggerGen;

namespace ContosoPizza.Extensions.Swagger;

public static class SwaggerExtensions
{
    public static void UseCustomGeneratorSchemaIds(this SwaggerNamespace.SwaggerGenOptions options)
    {
        options.CustomSchemaIds(schema =>
        {
            if (schema.IsGenericType)
            {
                string baseClassName = typeof(ContosoPizza.Models.BaseHttpResponse<object>).Name;
                // Set custom schema id for http response generic model
                if (schema.Name.Equals(baseClassName))
                {
                    // Retrieving the generic argument name 
                    // (BaseHttpResponse<Pizza>) in this case is the name of the class of Pizza model
                    string genericTypeName = schema.GenericTypeArguments.FirstOrDefault()?.Name ?? string.Empty;
                    return $"HttpResponse<{genericTypeName}>";
                }
            }

            // Check if the returned class type has the display name attribute, 
            // then put that name as the swagger schema id
            // otherwise check if the current name ends with "DTO" and replace it.
            return schema.GetCustomAttributes(false)
                  .OfType<DisplayNameAttribute>()
                  .FirstOrDefault()?.DisplayName ?? 
                    (schema.Name.EndsWith("DTO") 
                        ? schema.Name.Replace("DTO", string.Empty) 
                        : schema.Name);
        });
    }

    public static void SetupSecurityDefinitions(this SwaggerNamespace.SwaggerGenOptions options)
    {
        // Setup custom operator filter
        // set as secure API only when it has the authorize attribute.
        options.OperationFilter<SwaggerAuthorizationOperationFilter>();

        var securityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
            Reference = new OpenApiReference()
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        };

        options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

        // Note: The following configuration will make ALL APIs as protected!
        // options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        // {
        //     { securityScheme, new string[] {} }
        // });
    }
}