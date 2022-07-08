using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models;

public class RefreshTokens 
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Token { get; set; } = string.Empty;
    [Required]
    public bool Active { get; set; }
    [Required]
    public DateTime ExpiryDate { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
}