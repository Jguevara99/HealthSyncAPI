namespace Domain.Endpoint.Entities;

public enum Gender
{
    Male = 0,
    Female = 1
}

public class Patient : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
}
