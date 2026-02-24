using System.Security.Claims;
using Application.Models;
using MediatR;

namespace Application.Features.Users.CreateUser;

public record CreateUser(ClaimsPrincipal claims) : IRequest<TokenResponse>;