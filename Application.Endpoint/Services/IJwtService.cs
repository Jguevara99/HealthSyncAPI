using System.Security.Claims;

namespace Application.Endpoint.Services;

public interface IJwtService
{
    /// <summary> return the generated token and its expiry date </summary>
    public Tuple<string, DateTime> GenerateAccessToken(List<Claim> claims);
    public bool ValidateJwtToken(string token);
    public string GenerateRefreshToken();
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
