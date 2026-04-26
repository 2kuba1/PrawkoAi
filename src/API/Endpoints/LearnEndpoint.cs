using Application.Features.Learning.GetQuestionsForSet;
using Application.Features.Learning.GetStudyTopics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class LearnEndpoint
{
    public static void MapLearnEndpoints(this IEndpointRouteBuilder app)
    {
        var learnGroup = app.MapGroup("/api/learn");
        
        learnGroup.MapGet("/getStudyTopics", GetStudyTopics)
            .RequireAuthorization();
        
        learnGroup.MapGet("/getQuestionsForSet", GetQuestionsForSet)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetStudyTopics([FromQuery] Guid userId, [FromQuery] string category,[FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetStudyTopics(userId, category));
        return Results.Ok(results);
    }

    private static async Task<IResult> GetQuestionsForSet([FromQuery] string categoryTag,
        [FromQuery] string categoryType, [FromQuery] int setNumber, [FromQuery] string locale, [FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetQuestionsForSet(categoryTag, categoryType.ToUpper(), setNumber, locale.ToUpper()));
        return Results.Ok(results);
    }
}