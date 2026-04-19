using Application.Features.Answers.LogUserAnswersInSet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AnswerEndpoints
{
    public static void MapAnswerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answer/answerSet", LogUserAnswersInSet);
    }

    private static async Task<IResult> LogUserAnswersInSet([FromBody] List<AnswersInSet> questionSet, [FromServices] IMediator mediator)
    {
        await mediator.Send(new LogUserAnswersInSet());
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