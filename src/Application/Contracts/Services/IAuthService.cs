using Domain;
using Domain.Entities;

namespace Application.Contracts.Services;

public interface IAuthService
{
    string CreateToken(User user);
    string GenerateRefreshToken();
}