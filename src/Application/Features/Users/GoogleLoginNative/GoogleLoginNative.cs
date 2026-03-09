using Application.Models;
using MediatR;

namespace Application.Features.Users.GoogleLoginNative;

public record GoogleLoginNative(string Email) : IRequest<TokenResponse>;