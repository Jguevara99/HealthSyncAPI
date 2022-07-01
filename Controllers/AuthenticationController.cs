using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using ContosoPizza.DTOs;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JWT _jwtConfig;

    public AuthenticationController(UserManager<ApplicationUser> userManager,
                                    RoleManager<ApplicationRole> roleManager, IOptions<JWT> jwtOptions)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtConfig = jwtOptions.Value;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseHttpResponse))]
    public async Task<IActionResult> Login(LoginRequestDTO model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var tokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Secret));

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudience,
                expires: DateTime.Now.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return Ok(new Ok<LoginResponseDTO>("You're logged in successfully!",
                        new LoginResponseDTO()
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            username = model.Username,
                            roles = userRoles.ToList()
                        })
                    );
        }

        return NotFound(new NotFound("The user doesn't exists!"));
    }

    /// <summary>
    /// Create a new user as Guest
    /// </summary>
    /// <response code="200">Ok: ser created successfully</response>
    /// <response code="422">UnprocessableEntity: User creation failed</response>
    /// <response code="409">Conflict: User already exists!</response>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<RegisterResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(BaseHttpResponse<RegisterIdentityErrorResponse>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseHttpResponse))]
    public async Task<IActionResult> Register(RegisterRequestDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status409Conflict, 
                             new BaseHttpResponse("User already exists!", StatusCodes.Status409Conflict, false));

        ApplicationUser user = new ApplicationUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status422UnprocessableEntity,
                             new BaseHttpResponse<RegisterIdentityErrorResponse>("User creation failed! Please check the details...",
                                                  StatusCodes.Status422UnprocessableEntity, false,
                                                  new RegisterIdentityErrorResponse()
                                                  {
                                                      RequestedUser = model,
                                                      errors = result.Errors
                                                  }));

        return Ok(new Ok<RegisterResponseDTO>("User created successfully!",
                  new RegisterResponseDTO()
                  {
                      Id = user.Id,
                      Email = model.Email,
                      Username = model.Username
                  }));
    }
}