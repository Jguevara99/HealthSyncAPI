using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;

namespace Infrastructure.Endpoint.Repositories;

public class DepartmentsRepository : EfRepository<Department>, IDeparmentsRepository
{
    public DepartmentsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
