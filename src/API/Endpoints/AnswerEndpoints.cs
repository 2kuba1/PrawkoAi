using Application.Features.Answers.LogUserAnswersInSet;
using Application.Models.DTOs;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AnswerEndpoints
{
    public static void MapAnswerEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();
        
        var answerGroup = app.MapGroup("/api/v{version:apiVersion}/answer")
            .WithApiVersionSet(versionSet);
        
        answerGroup.MapPost("/answerSet", LogUserAnswersInSet);
    }

    private static async Task<IResult> LogUserAnswersInSet([FromBody] AnswersInSet questionSet, [FromServices] IMediator mediator)
    {
        var mappedAnswers = questionSet.Answers.Select(a => new UserSetAnswerDto
        (
            a.SelectedAnswerId,
            a.QuestionId,
            a.AnsweredAt
        )).ToList();
        
        await mediator.Send(new LogUserAnswersInSet(
            questionSet.UserId,
            mappedAnswers,
            questionSet.CategoryName.ToUpper()
        ));

        return Results.NoContent();
    }

    private record AnswersInSet(
        Guid UserId,
        List<UserSetAnswer> Answers,
        string CategoryName = "B"
        );

    private record UserSetAnswer(
        Guid? SelectedAnswerId,
        Guid QuestionId,
        DateTime AnsweredAt);
}