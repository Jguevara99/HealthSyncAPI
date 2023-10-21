namespace Domain.Endpoint.Entities
{
    public class Doctor : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid SpecialtyId { get; set; }
        public Guid DeparmentId { get; set; }
    }
}
