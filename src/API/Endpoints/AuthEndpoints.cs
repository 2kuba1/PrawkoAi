using Application.Features.Users.GoogleLogin;
using Application.Features.Users.GoogleLoginNative;
using Application.Features.Users.GuestLogin;
using Application.Features.Users.RefreshAuthToken;
using Application.Features.Users.RevokeRefreshTokens;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Google.Apis;
using Google.Apis.Auth;

namespace API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/account/login/google", GoogleLogin);
        app.MapGet("/api/account/login/google/callback", GoogleLoginCallback).WithName("GoogleLoginCallback");
        app.MapGet("/api/account/login/guest", GuestLogin);
        app.MapGet("/api/account/refresh-token", RefreshToken);
        app.MapDelete("/api/account/logout", Logout)
            .RequireAuthorization();
        app.MapPost("/api/account/login/google-native", GoogleLoginNative);
    }

    private static async Task<IResult> GoogleLoginNative([FromBody] GoogleLoginRequest request, [FromServices] IMediator mediator, [FromServices] IConfiguration configuration)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [configuration.GetValue<string>("Google:GoogleNativeAuthSecret")],
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            
            var tokens = await mediator.Send(new GoogleLoginNative(payload.Email));

            return Results.Ok(tokens);
        }
        catch (InvalidJwtException)
        {
            return Results.Unauthorized();
        }
    }

    private static IResult GoogleLogin([FromQuery] string returnUrl, HttpContext httpContext)
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = "/api/account/login/google/callback",
            Items =
            {
                ["returnUrl"] = returnUrl
            }
        };
        return Results.Challenge(props, ["Google"]);
    }

    private static async Task<IResult> GoogleLoginCallback(HttpContext httpContext,
        [FromServices] IMediator mediator)
    {
        var result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return Results.Unauthorized();

        var authenticateResult = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        var returnUrl = authenticateResult.Properties?.Items["returnUrl"];
        
        var tokens = await mediator.Send(new GoogleLogin(result.Principal), CancellationToken.None);
        
        var redirectTarget = $"{returnUrl}?token={tokens.AccessToken}&refreshToken={tokens.RefreshToken}";
        
        return Results.Ok(redirectTarget);
    }

    private static async Task<IResult> Logout([FromQuery] Guid userId, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new RevokeRefreshTokens(userId));
        return Results.Ok(result);
    }

    private static async Task<IResult> GuestLogin([FromQuery] string deviceId, [FromServices] IMediator mediator)
    {
        var tokens = await mediator.Send(new GuestLogin(deviceId));
        return Results.Ok(tokens);
    }

    private static async Task<IResult> RefreshToken([FromQuery] string refreshToken, [FromServices] IMediator mediator)
    {
        var newTokens = await mediator.Send(new RefreshAuthToken(refreshToken));
        return Results.Ok(newTokens);
    }

    private record GoogleLoginRequest(string IdToken);
}
 