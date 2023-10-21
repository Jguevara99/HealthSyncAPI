using Domain.Endpoint.Entities;
using System.Linq.Expressions;

namespace Domain.Endpoint.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task Add(RefreshToken refreshToken);
    Task Update(RefreshToken refreshTokens);
    Task<RefreshToken?> GetBy(Expression<Func<RefreshToken, bool>> expression);
    Task<List<RefreshToken>> GetActivesByUser(Guid UserId);
}
