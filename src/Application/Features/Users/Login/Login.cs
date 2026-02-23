using Application.Models;
using MediatR;

namespace Application.Features.Users.Login;

public record Login() : IRequest<TokenResponse>
{
    
}