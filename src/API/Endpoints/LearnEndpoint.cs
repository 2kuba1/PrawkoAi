using Application.Features.Learning.GetStudyTopics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class LearnEndpoint
{
    public static void MapLearnEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/learn/getStudyTopics", GetStudyTopics)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetStudyTopics([FromQuery] Guid userId, [FromQuery] string category,[FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetStudyTopics(userId, category));
        return Results.Ok(results);
    }
}