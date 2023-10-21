using Domain.Endpoint.Entities;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface ISpecialtiesRepository
{
    Task<Specialty> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Specialty>> ListAllAsync();
    Task<Specialty> AddAsync(Specialty entity);
    Task UpdateAsync(Specialty entity);
    Task DeleteAsync(Specialty entity);
    Task<List<Specialty>> AddAsync(List<Specialty> entity);
    Task<List<Specialty>> DeleteAsync(List<Specialty> entity);
    Task<List<Specialty>> UpdateAsync(List<Specialty> entity);
}
