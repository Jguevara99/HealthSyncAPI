using System.Net.Mime;
using ContosoPizza.DTOs;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<LoginResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseHttpResponse))]
    public async Task<IActionResult> Login(LoginRequestDTO model)
    {
        var response = await _authenticationService.Authenticate(model.Username, model.Password);
        return Ok(new Ok<LoginResponseDTO>("You're logged in successfully!", response));
    }

    /// <summary>
    /// Create a new user as Guest
    /// </summary>
    /// <response code="200">Ok: User created successfully</response>
    /// <response code="422">UnprocessableEntity: User creation failed</response>
    /// <response code="409">Conflict: User already exists!</response>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseHttpResponse<RegisterResponseDTO>))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(BaseHttpResponse<RegisterIdentityErrorResponse>))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(BaseHttpResponse))]
    public async Task<IActionResult> Register(RegisterRequestDTO model)
    {
        var response = await _authenticationService.RegisterUser(model);
        return Ok(new Ok<RegisterResponseDTO>("User created successfully!", response));
    }
}