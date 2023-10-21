using HealthSyncAPI.Application.Endpoints.DTOs;

namespace Application.Endpoint.Services;

public interface IAuthenticationService
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDTO> Authenticate(string username, string password);
        Task<RegisterResponseDTO> RegisterUser(RegisterRequestDTO requestModel);
        Task<JwtAuthDTO> RefreshToken(JwtRefreshDTO jwtRefreshModel);
        Task<bool> RevokeRefreshToken(string refreshToken);
    }
}
