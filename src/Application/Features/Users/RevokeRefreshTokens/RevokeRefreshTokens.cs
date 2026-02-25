using MediatR;

namespace Application.Features.Users.RevokeRefreshTokens;

public record RevokeRefreshTokens(Guid UserId) : IRequest<bool>;