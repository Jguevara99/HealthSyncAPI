using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Models;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;
    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(RefreshTokens refreshToken)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<RefreshTokens>> GetActivesByUser(Guid UserId)
    {
        return _dbContext.RefreshTokens.Where(rt => rt.Active && 
                                                    rt.RevokedAt == null && 
                                                    rt.UserId == UserId)
                                        .ToListAsync();
    }

    public Task<RefreshTokens?> GetBy(Expression<Func<RefreshTokens, bool>> expression)
    {
        var refreshToken = _dbContext.RefreshTokens.Where(expression).FirstOrDefault();
        return Task.FromResult(refreshToken);
    }

    public async Task Update(RefreshTokens refreshTokens)
    {
        _dbContext.RefreshTokens.Update(refreshTokens);
        await _dbContext.SaveChangesAsync();
    }
}

// TODO: Return meaningfull values per action juts to know if the operation was success & testing purpose
public interface IRefreshTokenRepository
{
    Task Add(RefreshTokens refreshToken);
    Task Update(RefreshTokens refreshTokens);
    Task<RefreshTokens?> GetBy(Expression<Func<RefreshTokens, bool>> expression);
    Task<List<RefreshTokens>> GetActivesByUser(Guid UserId);
}