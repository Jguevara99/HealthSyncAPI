using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthSyncAPI.Application.Endpoints.DTOs;

public class RegisterRequestDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponseDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class RegisterIdentityErrorResponse 
{
    public RegisterRequestDTO? RequestedUser { get; set; }
    public IEnumerable<IdentityError> errors { get; set; } = new List<IdentityError>();
}