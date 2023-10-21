using System.ComponentModel.DataAnnotations;

namespace HealthSyncAPI.Application.Endpoints.DTOs;

public class JwtAuthDTO
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiresIn { get; set; }
}

public class JwtRefreshDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}