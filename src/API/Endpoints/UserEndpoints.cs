using Application.Features.Users.GetStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
         var userGroup = app.MapGroup("/api/user");
        
         userGroup.MapGet("/stats", GetUserStats)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetUserStats([FromQuery] Guid userId, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetStats(userId));
        return Results.Ok(result);
    }
}