using System.ComponentModel;
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
                    return $"{genericTypeName}Response";
                }
            }

            // Check if the returned class type has the display name attribute, 
            // then put that name as the swagger schema id
            // otherwise put the default schema name.
            return schema.GetCustomAttributes(false)
                  .OfType<DisplayNameAttribute>()
                  .FirstOrDefault()?.DisplayName ?? schema.Name;
        });
    }
}