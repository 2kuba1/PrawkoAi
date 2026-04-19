using Application.Features.Answers.LogUserAnswersInSet;
using Application.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AnswerEndpoints
{
    public static void MapAnswerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answer/answerSet", LogUserAnswersInSet);
    }

    private static async Task<IResult> LogUserAnswersInSet([FromBody] AnswersInSet questionSet, [FromServices] IMediator mediator)
    {
        var mappedAnswers = questionSet.Answers.Select(a => new UserSetAnswerDto
        (
            a.QuestionId,
            a.SelectedAnswerId,
            a.AnsweredAt
        )).ToList();

        await mediator.Send(new LogUserAnswersInSet(
            questionSet.UserId,
            mappedAnswers
        ));

        return Results.NoContent();
    }

    private record AnswersInSet(
        Guid UserId,
        List<UserSetAnswer> Answers
        );

    private record UserSetAnswer(
        Guid SelectedAnswerId,
        Guid QuestionId,
        DateTime AnsweredAt);
}