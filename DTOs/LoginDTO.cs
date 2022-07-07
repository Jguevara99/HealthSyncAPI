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
    public string username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> roles { get; set; } = new List<string>();
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiresIn { get; set; }
}