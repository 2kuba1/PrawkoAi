using Application.Features.Users.GoogleLogin;
using Application.Features.Users.RevokeRefreshTokens;
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
        app.MapDelete("/api/account/logout", Logout);
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

        var tokens = await mediator.Send(new GoogleLogin(result.Principal), CancellationToken.None);
        
        return Results.Ok(tokens);
    }

    private static async Task<IResult> Logout([FromServices] IMediator mediator, [FromQuery]string userId)
    {
        var result = await mediator.Send(new RevokeRefreshTokens(Guid.Parse(userId)));
        return Results.Ok(result);
    }
}