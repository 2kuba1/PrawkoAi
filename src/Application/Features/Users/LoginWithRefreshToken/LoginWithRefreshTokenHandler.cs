using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using MediatR;

namespace Application.Features.Users.LoginWithRefreshToken;

public class LoginWithRefreshTokenHandler : IRequestHandler<LoginWithRefreshToken, TokenResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuthService _authService;

    public LoginWithRefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IAuthService authService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _authService = authService;
    }
    
    public async Task<TokenResponse> Handle(LoginWithRefreshToken request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetUsersRefreshToken(request.RefreshToken);
        
        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            throw new ApplicationException("The refresh token has expired");
        
        var accessToken = _authService.CreateToken(refreshToken.User);

        refreshToken.Token = _authService.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(30);

        await _refreshTokenRepository.UpdateAsync(refreshToken);
        
        return new TokenResponse(accessToken, refreshToken.Token);
    }
}