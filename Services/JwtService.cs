using System.Security.Claims;
using ContosoPizza.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ContosoPizza.Services;

public class JwtService : IJwtService
{
    private readonly JWT _jwtOptions;
    public JwtService(IOptions<JWT> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }


    public Tuple<string, DateTime> GenerateAccessToken(List<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));

        var securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.ValidIssuer,
            audience: _jwtOptions.ValidAudience,
            expires: GetExpiryDateFromOptions(_jwtOptions.ExpiryOptions),
            claims: claims,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        );

        string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return new Tuple<string, DateTime>(token, securityToken.ValidTo);
    }

    /// <summary>Return the configured jwt expiry date in the appsettings. Default Expiry Date: ONE DAY</summary>
    DateTime GetExpiryDateFromOptions(JwtExpiryOptions? jwtExpiryOptions)
    {
        try
        {
            if (jwtExpiryOptions is null)
                return DateTime.Now.AddDays(JwtExpiryOptions.DefaultExpirationTimeInDays);

            switch (jwtExpiryOptions.SetIn)
            {
                case JwtExpiryOptions.TimeOptions.Days:
                    return DateTime.Now.AddDays(jwtExpiryOptions.ExpiresIn);
                case JwtExpiryOptions.TimeOptions.Hours:
                    return DateTime.Now.AddHours(jwtExpiryOptions.ExpiresIn);
                case JwtExpiryOptions.TimeOptions.Minutes:
                    return DateTime.Now.AddMinutes(jwtExpiryOptions.ExpiresIn);
                default:
                    return DateTime.Now.AddDays(JwtExpiryOptions.DefaultExpirationTimeInDays);
            }
        }
        catch
        {
            return DateTime.Now.AddDays(JwtExpiryOptions.DefaultExpirationTimeInDays);
        }
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public bool ValidateJwtToken(string token)
    {
        throw new NotImplementedException();
    }
}

public interface IJwtService
{
    /// <summary> return the generated token and its expiry date </summary>
    public Tuple<string, DateTime> GenerateAccessToken(List<Claim> claims);
    public bool ValidateJwtToken(string token);
    public string GenerateRefreshToken();
}