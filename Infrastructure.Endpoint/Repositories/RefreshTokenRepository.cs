using Domain.Endpoint.Entities;
using Domain.Endpoint.Interfaces.Repositories;
using System.Linq.Expressions;

namespace Infrastructure.Endpoint.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;
    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(RefreshToken refreshToken)
    {
        _dbContext.RefreshToken.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<RefreshToken>> GetActivesByUser(Guid UserId)
    {
        return _dbContext.RefreshToken.Where(rt => rt.Active &&
                                                    rt.RevokedAt == null &&
                                                    rt.UserId == UserId)
                                        .ToListAsync();
    }

    public Task<RefreshToken?> GetBy(Expression<Func<RefreshToken, bool>> expression)
    {
        var refreshToken = _dbContext.RefreshToken.Where(expression).FirstOrDefault();
        return Task.FromResult(refreshToken);
    }

    public async Task Update(RefreshToken RefreshToken)
    {
        _dbContext.RefreshToken.Update(RefreshToken);
        await _dbContext.SaveChangesAsync();
    }
}
