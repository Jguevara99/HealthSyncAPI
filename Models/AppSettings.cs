using System.Text.Json.Serialization;

namespace ContosoPizza.Models;

public class AppSettings
{
    public Logging? Logging { get; set; }
    public string AllowedHosts { get; set; } = string.Empty;
    public ConnectionStrings? ConnectionStrings { get; set; }
    public JWT? JWT { get; set; }
}

public class Logging
{
    public LogLevel? LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; } = string.Empty;

    [JsonPropertyName("Microsoft.AspNetCore")]
    public string MicrosoftAspNetCore { get; set; } = string.Empty;
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

public class JWT
{
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
}