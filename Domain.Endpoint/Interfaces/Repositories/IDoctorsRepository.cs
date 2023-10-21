using Domain.Endpoint.Entities;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface IDoctorsRepository
{
    Task<Doctor> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Doctor>> ListAllAsync();
    Task<Doctor> AddAsync(Doctor entity);
    Task UpdateAsync(Doctor entity);
    Task DeleteAsync(Doctor entity);
    Task<List<Doctor>> AddAsync(List<Doctor> entity);
    Task<List<Doctor>> DeleteAsync(List<Doctor> entity);
    Task<List<Doctor>> UpdateAsync(List<Doctor> entity);
}
