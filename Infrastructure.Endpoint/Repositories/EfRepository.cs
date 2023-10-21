using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using Infrastructure.Endpoint.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Endpoint.Repositories;

public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _dbContext;

    public EfRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    //public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    //{
    //    var obj = ApplySpecification(spec);
    //    var sql = obj.ToSql();
    //    //Console.WriteLine (sql);
    //    return await obj.ToListAsync();
    //    //return await ApplySpecification(spec).ToListAsync();
    //}
    //public async Task<IReadOnlyList<T>> ListIgnoreFiltersAsync(ISpecification<T> spec)
    //{
    //    var obj = ApplySpecification(spec).IgnoreQueryFilters();
    //    return await obj.ToListAsync();
    //}

    //public async Task<int> CountAsync(ISpecification<T> spec)
    //{
    //    return await ApplySpecification(spec).CountAsync();
    //}

    public async Task<T> AddAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<List<T>> AddAsync(List<T> entity)
    {
        foreach (var item in entity)
        {
            _dbContext.Set<T>().Add(item);
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<List<T>> DeleteAsync(List<T> entity)
    {
        foreach (var item in entity)
        {
            _dbContext.Set<T>().Remove(item);
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<List<T>> UpdateAsync(List<T> entity)
    {
        foreach (var item in entity)
        {
            _dbContext.Entry(item).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    //public IQueryable<T> ApplySpecification(ISpecification<T> spec)
    //{
    //    var obj = SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
    //    return obj;
    //}
}