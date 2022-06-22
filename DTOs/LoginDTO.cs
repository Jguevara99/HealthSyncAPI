using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.DTOs;

public class LoginRequestDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
}