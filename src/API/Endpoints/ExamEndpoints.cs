using Application.Features.Exam.ExamAnswer;
using Application.Features.Exam.FinishExam;
using Application.Features.Exam.GetUserExamSessionResults;
using Application.Features.Exam.GetUserExamsSessionHistory;
using Application.Features.Exam.StartExam;
using Application.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ExamEndpoints
{
    public static void MapExamEndpoints(this IEndpointRouteBuilder app)
    {
        var examGroup = app.MapGroup("/api/exam");
        
        examGroup.MapGet("/start", StartExam)
            .RequireAuthorization();
        
        examGroup.MapPost("/answer", AnswerToExamQuestion)
            .RequireAuthorization();

        examGroup.MapPut("/finish", FinishExam)
            .RequireAuthorization();

        examGroup.MapGet("/userHistory", GetUserExamSessionsHistory)
            .RequireAuthorization();

        examGroup.MapGet("/examResults", GetUserExamSessionResults)
            .RequireAuthorization();
    }

    private static async Task<IResult> StartExam([FromQuery]Guid userId, [FromQuery] string category, [FromQuery] string? locale, [FromServices] IMediator mediator)
    {
        var questions = await mediator.Send(new StartExam(userId, category, locale));
        return Results.Ok(questions);
    }

    private static async Task<IResult> AnswerToExamQuestion([FromBody]AnswerToQuestionRequest request, [FromServices] IMediator mediator)
    {
        await mediator.Send(new ExamAnswer(request.ExamSessionId, request.QuestionId, request.SelectedAnswerId, request.UserId));
        return Results.NoContent();
    }

    private static async Task<IResult> FinishExam([FromBody] FinishExamSession finishExamSession,
        [FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new FinishExam(finishExamSession.UserId, finishExamSession.ExamSessionId, finishExamSession.Locale, finishExamSession.Answers));
        return Results.Ok(results);
    }

    private static async Task<IResult> GetUserExamSessionsHistory([FromQuery] Guid userId, [FromQuery] int pageNumber,[FromQuery] int pageSize,[FromServices] IMediator mediator)
    {
        if(pageSize > 15)
            return Results.BadRequest("PageSize must be less than 15");
        
        var results = await mediator.Send(new GetUserExamsSessionHistory(userId, pageNumber, pageSize));
        return Results.Ok(results);
    }

    private static async Task<IResult> GetUserExamSessionResults([FromQuery] Guid userId,
        [FromQuery] Guid examSessionId, [FromQuery] string locale, [FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetUserExamSessionResults(userId, examSessionId, locale)); 
        return Results.Ok(results);
    }   
    
    private record FinishExamSession(
        Guid UserId,
        Guid ExamSessionId,
        string Locale,
        List<UserAnswerSubmissionDto> Answers);

    private record AnswerToQuestionRequest(
        Guid QuestionId,
        Guid? SelectedAnswerId,
        Guid UserId,
        Guid ExamSessionId);
    
}