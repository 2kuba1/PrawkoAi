using System.Security.Claims;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Users.GoogleLogin;

internal sealed class GoogleLoginHandler : IRequestHandler<GoogleLogin, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRoleRepository _roleRepository;

    public GoogleLoginHandler(IUserRepository userRepository, IAuthService authService, IRefreshTokenRepository refreshTokenRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
        _roleRepository = roleRepository;
    }
    
    public async Task<TokenResponse> Handle(GoogleLogin request, CancellationToken cancellationToken)
    {
        if(request.Claims is null)
            throw new ArgumentException("Claims cannot be null");

        var email = request.Claims.FindFirst(ClaimTypes.Email).Value;
        
        if(email is null)
            throw new ArgumentException("Claims cannot be null");

        var user = await _userRepository.FindUserByEmailAsync(email);

        if (user is null)
        {
            var role = await _roleRepository.GetRoleByName("User");
            
            if(role is null)
                throw new KeyNotFoundException($"Role does not exist");
            
            var newUser = new User()
            {
                Email = email,
                DeviceId = "123",
                RoleId =  role.Id,
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