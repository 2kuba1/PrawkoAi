using System.Security.Claims;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUser, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public CreateUserHandler(IUserRepository userRepository, IAuthService authService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<TokenResponse> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        if(request.claims is null)
            throw new ArgumentException("Claims cannot be null");

        var email = request.claims.FindFirst(ClaimTypes.Email).Value;
        
        if(email is null)
            throw new ArgumentException("Claims cannot be null");

        var user = await _userRepository.FindUserByEmailAsync(email);

        if (user is null)
        {
            var newUser = new User()
            {
                Email = email,
                DeviceId = "123",
            };

            await _userRepository.CreateAsync(newUser);
            
            user = newUser;
        }

        var token = _authService.CreateToken(user);

        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _authService.GenerateRefreshToken(),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(30)
        };
        
        await _refreshTokenRepository.CreateAsync(refreshToken);
        
        return new TokenResponse(token, refreshToken.Token);
    }
}