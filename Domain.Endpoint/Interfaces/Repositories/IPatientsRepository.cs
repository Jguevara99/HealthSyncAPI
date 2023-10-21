using Domain.Endpoint.Entities;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface IPatientsRepository
{
    Task<Patient> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Patient>> ListAllAsync();
    Task<Patient> AddAsync(Patient entity);
    Task UpdateAsync(Patient entity);
    Task DeleteAsync(Patient entity);
    Task<List<Patient>> AddAsync(List<Patient> entity);
    Task<List<Patient>> DeleteAsync(List<Patient> entity);
    Task<List<Patient>> UpdateAsync(List<Patient> entity);
}
