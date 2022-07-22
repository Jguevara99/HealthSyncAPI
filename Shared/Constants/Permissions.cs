using System.Reflection;

namespace ContosoPizza.Shared.Constants;

public static class Permissions
{
    public static List<string> GenerateBasePermissions4Module(string module) => new List<string>(){
        $"Permissions:{module}:Create",
        $"Permissions:{module}:Read",
        $"Permissions:{module}:Update",
        $"Permissions:{module}:Delete",
    };

    public static class Users
    {
        public const string Create = "Permissions:Users:Create";
        public const string Read = "Permissions:Users:Read";
        public const string Update = "Permissions:Users:Update";
        public const string Delete = "Permissions:Users:Delete";
    }

    public static class Pizzas
    {
        public const string Create = "Permissions:Pizzas:Create";
        public const string Read = "Permissions:Pizzas:Read";
        public const string Update = "Permissions:Pizzas:Update";
        public const string Delete = "Permissions:Pizzas:Delete";
    }

    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions)
                                    .GetNestedTypes()
                                    .SelectMany(types =>
                                        types.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                    ))
        {
            string propertyValueString = prop.GetValue(null)?.ToString() ?? string.Empty;
            if (!String.IsNullOrEmpty(propertyValueString))
            {
                permissions.Add(propertyValueString);
            }
        }   
        return permissions;
    }
}