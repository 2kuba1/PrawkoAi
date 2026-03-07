using System.Security.Claims;
using Application.Contracts.Repositories;
using Application.Shared;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Users.RevokeRefreshTokens;

internal sealed class RevokeRefreshTokensHandler : IRequestHandler<RevokeRefreshTokens, bool>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RevokeRefreshTokensHandler(IRefreshTokenRepository refreshTokenRepository, IHttpContextAccessor httpContextAccessor)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<bool> Handle(RevokeRefreshTokens request, CancellationToken cancellationToken)
    {
        if(request.UserId != Utils.GetCurrentUserId(_httpContextAccessor))
            throw new UnauthorizedException("You are not authorized to revoke this refresh tokens");
        
        await _refreshTokenRepository.RemoveUserRefreshTokens(request.UserId);

        return true;
    }
}