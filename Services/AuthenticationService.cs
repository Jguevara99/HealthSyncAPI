using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ContosoPizza.DTOs;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ContosoPizza.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly JWT _jwtOptions;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IRefreshTokenRepository refreshTokenRepository,
                                 IJwtService jwtService,
                                 IOptions<JWT> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LoginResponseDTO> Authenticate(string usernameOrEmail, string password)
    {
        if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            throw new HttpException("The username and password are required!", StatusCodes.Status400BadRequest);

        ApplicationUser? user = await (IsValidEmail(usernameOrEmail)
                                        ? _userManager.FindByEmailAsync(usernameOrEmail)
                                        : _userManager.FindByNameAsync(usernameOrEmail));

        if (user is null)
            throw new HttpException("The user doesn't exists!", StatusCodes.Status404NotFound);

        bool wrongPassword = await _userManager.CheckPasswordAsync(user, password);

        if (!wrongPassword)
            throw new HttpException("The password is wrong!", StatusCodes.Status400BadRequest);

        var claimsRoles = await GetUserClaimsWithRoles(user);

        Tuple<string, DateTime> tokenInfo = _jwtService.GenerateAccessToken(claimsRoles.Item1);
        string refreshToken = _jwtService.GenerateRefreshToken();
        await CreateRefreshToken(user.Id, refreshToken);

        return new LoginResponseDTO
        {
            username = user.UserName,
            Email = user.Email,
            roles = claimsRoles.Item2,
            Jwt = new JwtAuthDTO
            {
                Token = tokenInfo.Item1,
                TokenExpiresIn = tokenInfo.Item2,
                RefreshToken = refreshToken
            }
        };
    }

    public async Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestModel)
    {
        var user = await _userManager.FindByNameAsync(requestModel.Username);
        if (user is not null)
            throw new HttpException("The username already exists!", StatusCodes.Status409Conflict);

        ApplicationUser newUser = new ApplicationUser
        {
            Email = requestModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = requestModel.Username
        };

        IdentityResult indetityResult = await _userManager.CreateAsync(newUser, requestModel.Password);

        if (!indetityResult.Succeeded)
            throw new HttpException("User creation failded!",
                                    StatusCodes.Status422UnprocessableEntity,
                                    new RegisterIdentityErrorResponse
                                    {
                                        RequestedUser = requestModel,
                                        errors = indetityResult.Errors
                                    });

        return new RegisterResponseDTO
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Username = newUser.UserName
        };
    }

    public async Task<JwtAuthDTO> RefreshToken(JwtRefreshDTO jwtRefreshModel)
    {
        var refreshToken = await _refreshTokenRepository.GetBy(r => r.Token == jwtRefreshModel.RefreshToken);
        if (refreshToken is null || refreshToken.ExpiryDate <= DateTime.UtcNow)
            throw new HttpException(
                        "Invalid refresh token: it doesn't exists or has expired",
                        StatusCodes.Status403Forbidden);

        ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(jwtRefreshModel.Token);
        ApplicationUser? user = await _userManager.GetUserAsync(principal);

        if (user is null)
            throw new HttpException("Invalid token: User not found", StatusCodes.Status404NotFound);

        if (!user.Id.Equals(refreshToken.UserId))
            throw new HttpException("The token & refresh token don't share the same signature", StatusCodes.Status403Forbidden);

        if (!refreshToken.Active)
        {
            var activeRefreshTokens = await _refreshTokenRepository.GetActivesByUser(refreshToken.UserId);
            // remove all sessions...
            foreach (var activeTokens in activeRefreshTokens)
            {
                activeTokens.Active = false;
                activeTokens.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.Update(activeTokens);
            }

            throw new HttpException("The refresh token has already been used!", StatusCodes.Status403Forbidden);
        }

        refreshToken.Active = false;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.Update(refreshToken);

        List<Claim> claims = (await GetUserClaimsWithRoles(user)).Item1;
        var tokenInfo = _jwtService.GenerateAccessToken(claims);

        string newRefreshToken = _jwtService.GenerateRefreshToken();
        await CreateRefreshToken(user.Id, newRefreshToken);

        return new JwtAuthDTO
        {
            Token = tokenInfo.Item1,
            RefreshToken = newRefreshToken,
            TokenExpiresIn = tokenInfo.Item2
        };
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }

    private async Task<RefreshTokens> CreateRefreshToken(Guid userId, string refreshTokenValue)
    {
        var refreshToken = new RefreshTokens
        {
            UserId = userId,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(2),
            Token = refreshTokenValue
        };

        await _refreshTokenRepository.Add(refreshToken);
        return refreshToken;
    }

    private async Task<Tuple<List<Claim>, List<string>>> GetUserClaimsWithRoles(ApplicationUser user)
    {
        string userId = user.Id.ToString();
        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Sid, userId),
            new Claim(JwtRegisteredClaimNames.Sub, userId)
        };

        foreach (string role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        return new Tuple<List<Claim>, List<string>>(authClaims, userRoles.ToList());
    }
}

public interface IAuthenticationService
{
    Task<LoginResponseDTO> Authenticate(string username, string password);
    Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestModel);
    Task<JwtAuthDTO> RefreshToken(JwtRefreshDTO jwtRefreshModel);
}