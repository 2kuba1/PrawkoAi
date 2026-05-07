using Application.Features.Users.GetStats;
using Application.Features.Users.WorstPerformingCategory;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();
        
         var userGroup = app.MapGroup("/api/v{version:apiVersion}/user")
             .WithApiVersionSet(versionSet);
        
         userGroup.MapGet("/stats", GetUserStats)
            .RequireAuthorization();
         
         userGroup.MapGet("/getUserWorstPerformingCategory", GetUserWorstPerformingCategory)
             .RequireAuthorization();
    }

    private static async Task<IResult> GetUserStats([FromQuery] Guid userId, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetStats(userId));
        return Results.Ok(result);
    }
    
    private static async Task<IResult> GetUserWorstPerformingCategory([FromQuery] Guid userId,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new WorstPerformingCategory(userId));
        return Results.Ok(result);
    }
}