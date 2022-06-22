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
                            Token = new JwtSecurityTokenHandler().WriteToken(token)
                        })
                    );
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

        ApplicationUser user = new ApplicationUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        return Ok(new { Status = "Success", Message = "User created successfully!" });
    }
}