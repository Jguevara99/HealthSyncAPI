using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;

namespace Infrastructure.Endpoint.Repositories
{
    public class SpecialtiesRepository : EfRepository<Specialty>, ISpecialtiesRepository
    {
        public SpecialtiesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
