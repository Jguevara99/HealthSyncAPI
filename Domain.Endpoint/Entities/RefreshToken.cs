using System.ComponentModel.DataAnnotations;

namespace Domain.Endpoint.Entities;

public class RefreshToken
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
    //public virtual User User { get; set; } = default!;
}
