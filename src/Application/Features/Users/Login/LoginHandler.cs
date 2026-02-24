using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Users.Login;

public class LoginHandler : IRequestHandler<Login, TokenResponse>
{
    private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    public LoginHandler(IAuthService authService, IRefreshTokenRepository refreshTokenRepository)
    {
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<TokenResponse> Handle(Login request, CancellationToken cancellationToken)
    {
        var token = _authService.CreateToken(request.user);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = request.user.Id,
            Token = _authService.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(30)
        };
        
        await _refreshTokenRepository.CreateAsync(refreshToken);
        
        return new TokenResponse(token, refreshToken.Token);
    }
}