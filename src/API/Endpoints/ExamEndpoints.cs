using Application.Features.Exam.ExamAnswer;
using Application.Features.Exam.StartExam;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ExamEndpoints
{
    public static void MapExamEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/exam/start", StartExam)
            .RequireAuthorization();
        
        app.MapPost("api/exam/answer", AnswerToExamQuestion)
            .RequireAuthorization();
    }

    private static async Task<IResult> StartExam([FromQuery]string userId, [FromServices] IMediator mediator)
    {
        var questions = await mediator.Send(new StartExam(userId));
        return Results.Ok(questions);
    }

    private static async Task<IResult> AnswerToExamQuestion([FromBody] AnswerToQuestionRequest request, [FromServices] IMediator mediator)
    {
        await mediator.Send(new ExamAnswer(request.ExamSessionId, request.QuestionId, request.SelectedAnswerId, request.UserId));
        return Results.Ok();
    }

    private record AnswerToQuestionRequest(
        Guid QuestionId,
        Guid SelectedAnswerId,
        Guid UserId,
        Guid ExamSessionId);
}