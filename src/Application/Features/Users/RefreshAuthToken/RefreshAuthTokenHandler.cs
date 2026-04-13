using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Users.RefreshAuthToken;

internal sealed class RefreshAuthTokenHandler : IRequestHandler<RefreshAuthToken, TokenResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public RefreshAuthTokenHandler(IRefreshTokenRepository refreshTokenRepository, IAuthService authService, IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _authService = authService;
        _configuration = configuration;
    }
    
    public async Task<TokenResponse> Handle(RefreshAuthToken request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetUsersRefreshToken(request.RefreshToken);
        
        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            throw new RefreshTokenExpiredException("The refresh token has expired");
        
        var accessToken = _authService.CreateToken(refreshToken.User);

        refreshToken.Token = _authService.GenerateRefreshToken();
        refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenExpirationTimeInDays"));

        await _refreshTokenRepository.UpdateAsync(refreshToken);
        
        return new TokenResponse(accessToken, refreshToken.Token);
    }
}