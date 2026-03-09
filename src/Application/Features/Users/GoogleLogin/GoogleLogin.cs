using System.Security.Claims;
using Application.Models;
using MediatR;

namespace Application.Features.Users.GoogleLogin;

public record GoogleLogin(ClaimsPrincipal Claims, string DeviceId) : IRequest<TokenResponse>;