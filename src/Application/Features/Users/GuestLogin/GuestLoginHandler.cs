using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain;
using Domain.Entities;
using MediatR;

namespace Application.Features.Users.GuestLogin;

internal sealed class GuestLoginHandler : IRequestHandler<GuestLogin, TokenResponse>
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    
    public GuestLoginHandler(IAuthService authService, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<TokenResponse> Handle(GuestLogin request, CancellationToken cancellationToken)
    {
        var guest = await _userRepository.FindUserByDeviceIdAsync(request.GuestDeviceId);

        if (guest is null)
            guest = await _userRepository.CreateNewGuestAsync(request.GuestDeviceId);
        
        var token = _authService.CreateToken(guest);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = guest.Id,
            Token = _authService.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(30)
        };
        
        await _refreshTokenRepository.CreateAsync(refreshToken);
        
        return new TokenResponse(token, refreshToken.Token);
    }
}