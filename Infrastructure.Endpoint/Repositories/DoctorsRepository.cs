using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;

namespace Infrastructure.Endpoint.Repositories
{
    public class DoctorsRepository : EfRepository<Doctor>, IDoctorsRepository
    {
        public DoctorsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
