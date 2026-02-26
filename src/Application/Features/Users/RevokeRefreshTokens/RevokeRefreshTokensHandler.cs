using System.Security.Claims;
using Application.Contracts.Repositories;
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
        if(request.UserId != GetCurrentUserId())
            throw new ApplicationException("You are not authorized to revoke this refresh tokens");
        
        await _refreshTokenRepository.RemoveUserRefreshTokens(request.UserId);

        return true;
    }

    private Guid? GetCurrentUserId()
    {
        return Guid.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId) ? userId : null;
    }
}