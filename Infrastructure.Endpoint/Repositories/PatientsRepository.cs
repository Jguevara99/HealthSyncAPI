using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;

namespace Infrastructure.Endpoint.Repositories;

public class PatientsRepository : EfRepository<Patient>, IPatientsRepository
{
    public PatientsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}