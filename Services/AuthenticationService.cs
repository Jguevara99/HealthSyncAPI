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
    private readonly IJwtService _jwtService;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IJwtService jwtService,
                                 IOptions<JWT> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
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

        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (string role in userRoles)
            authClaims.Add(new Claim(ClaimTypes.Role, role));

        Tuple<string, DateTime> tokenInfo = _jwtService.GenerateAccessToken(authClaims);

        return new LoginResponseDTO
        {
            username = user.UserName,
            Email = user.Email,
            Token = tokenInfo.Item1,
            TokenExpiresIn = tokenInfo.Item2,
            roles = userRoles.ToList()
        };
    }

    public async Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestModel)
    {
        var user = await _userManager.FindByNameAsync(requestModel.Username);
        if (user is null)
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

    bool IsValidEmail(string email)
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
}

public interface IAuthenticationService
{
    Task<LoginResponseDTO> Authenticate(string username, string password);
    Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestModel);
}