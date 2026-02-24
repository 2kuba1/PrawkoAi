using Application.Features.Users.CreateUser;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/account/login/google", GoogleLogin);
        app.MapGet("/api/account/login/google/callback", GoogleLoginCallback).WithName("GoogleLoginCallback");
    }

    private static IResult GoogleLogin(
        [FromQuery]string? returnUrl,
        HttpContext httpContext)
    {
        return Results.Challenge(new AuthenticationProperties()
        {
            RedirectUri = "/api/account/login/google/callback",
        },["Google"]);
    }

    private static async Task<IResult> GoogleLoginCallback(HttpContext httpContext,
        [FromServices]IMediator mediator)
    {
        var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return Results.Unauthorized();

        var tokens = await mediator.Send(new CreateUser(result.Principal), CancellationToken.None);
        
        return Results.Ok(tokens);
    }

}