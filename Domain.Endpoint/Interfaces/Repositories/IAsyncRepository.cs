using Domain.Endpoint.Entities;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface IAsyncRepository<T> where T : BaseEntity
{
    //IQueryable<T> ApplySpecification(ISpecification<T> spec);
    Task<T> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListAllAsync();
    //Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    //Task<IReadOnlyList<T>> ListIgnoreFiltersAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    //Task<int> CountAsync(ISpecification<T> spec);
    Task<List<T>> AddAsync(List<T> entity);
    Task<List<T>> DeleteAsync(List<T> entity);
    Task<List<T>> UpdateAsync(List<T> entity);
}
