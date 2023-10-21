using Application.Endpoint.Services;
using Infrastructure.Endpoint.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Endpoint.Services;

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
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public bool ValidateJwtToken(string token)
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret)),
                ValidateLifetime = false, // don't validate token's expiration date
                ValidAudience = _jwtOptions.ValidAudience,
                ValidIssuer = _jwtOptions.ValidIssuer
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null || !jwtSecurityToken
                                                .Header.Alg.Equals(
                                                    SecurityAlgorithms.HmacSha256Signature,
                                                    StringComparison.InvariantCultureIgnoreCase))
                throw new HttpException("Invalid Token", StatusCodes.Status400BadRequest);

            return principal;
        }
        catch (ArgumentException)
        {
            throw new HttpException("Invalid token!", StatusCodes.Status400BadRequest);
        }
    }
}
