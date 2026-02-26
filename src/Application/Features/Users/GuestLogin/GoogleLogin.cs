using Application.Models;
using MediatR;

namespace Application.Features.Users.GuestLogin;

public record GuestLogin(string GuestDeviceId) : IRequest<TokenResponse>
{
    
}