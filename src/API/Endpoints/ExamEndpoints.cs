using Application.Features.Exam.StartExam;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ExamEndpoints
{
    public static void MapExamEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/exam/start", StartExam);
    }

    private static async Task<IResult> StartExam([FromServices] IMediator mediator)
    {
        var questions = await mediator.Send(new StartExam());
        return Results.Ok(questions);
    }
}