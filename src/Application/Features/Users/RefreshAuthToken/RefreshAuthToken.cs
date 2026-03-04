using Application.Models;
using MediatR;

namespace Application.Features.Users.RefreshAuthToken;

public record RefreshAuthToken(string RefreshToken) : IRequest<TokenResponse>;