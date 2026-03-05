using Application.Features.Exam.ExamAnswer;
using Application.Features.Exam.FinishExam;
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
        
        app.MapPost("/api/exam/answer", AnswerToExamQuestion)
            .RequireAuthorization();

        app.MapPut("/api/exam/finish", FinishExam)
            .RequireAuthorization();
    }

    private static async Task<IResult> StartExam([FromQuery]string userId, [FromServices] IMediator mediator)
    {
        var questions = await mediator.Send(new StartExam(userId));
        return Results.Ok(questions);
    }

    private static async Task<IResult> AnswerToExamQuestion([FromBody]AnswerToQuestionRequest request, [FromServices] IMediator mediator)
    {
        await mediator.Send(new ExamAnswer(request.ExamSessionId, request.QuestionId, request.SelectedAnswerId, request.UserId));
        return Results.Ok();
    }

    private static async Task<IResult> FinishExam([FromBody] FinishExamSession finishExamSession,
        [FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new FinishExam(finishExamSession.UserId, finishExamSession.ExamSessionId));
        return Results.Ok(results);
    }
    
    private record FinishExamSession(
        Guid UserId,
        Guid ExamSessionId);

    private record AnswerToQuestionRequest(
        Guid QuestionId,
        Guid SelectedAnswerId,
        Guid UserId,
        Guid ExamSessionId);
}