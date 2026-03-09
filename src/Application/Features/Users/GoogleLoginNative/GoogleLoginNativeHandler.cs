using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Users.GoogleLoginNative;

internal sealed class GoogleLoginNativeHandler : IRequestHandler<GoogleLoginNative, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuthService _authService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public GoogleLoginNativeHandler(IUserRepository userRepository, IRoleRepository roleRepository, IAuthService authService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<TokenResponse> Handle(GoogleLoginNative request, CancellationToken cancellationToken)
    {
        if(request.Email is null)
            throw new NullParametersException("Claims cannot be null");

        var user = await _userRepository.FindUserByEmailAsync(request.Email);

        if (user is null)
        {
            var role = await _roleRepository.GetRoleByName("User");
            
            if(role is null)
                throw new NotFoundException($"Role does not exist");
            
            var newUser = new User()
            {
                Email = request.Email,
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