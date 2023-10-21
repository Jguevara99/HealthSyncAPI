using Domain.Endpoint.Entities;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface IDeparmentsRepository
{
    Task<Department> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Department>> ListAllAsync();
    Task<Department> AddAsync(Department entity);
    Task UpdateAsync(Department entity);
    Task DeleteAsync(Department entity);
    Task<List<Department>> AddAsync(List<Department> entity);
    Task<List<Department>> DeleteAsync(List<Department> entity);
    Task<List<Department>> UpdateAsync(List<Department> entity);
}
