using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Users.Login;

public record Login(User user) : IRequest<TokenResponse>
{
    
}