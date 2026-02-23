using Application.Models;
using MediatR;

namespace Application.Features.Users.LoginWithRefreshToken;

public record LoginWithRefreshToken(string RefreshToken) : IRequest<TokenResponse>;