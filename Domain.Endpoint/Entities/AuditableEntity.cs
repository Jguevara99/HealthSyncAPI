using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Endpoint.Entities
{
    public interface IHaveCreationData
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public interface IHaveUpdateData
    {
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }

    public class AuditableEntity : BaseEntity, IHaveCreationData, IHaveUpdateData
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
