using Application.Features.Answers.AnswerToQuestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AnswerEndpoints
{
    public static void MapAnswerEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/answers/answerToQuestion", AnswerToQuestion)
            .RequireAuthorization();
    }

    private static async Task<IResult> AnswerToQuestion([FromBody] AnswerToQuestionRequest request, [FromServices] IMediator mediator)
    {
        await mediator.Send(new AnswerToQuestion(request.QuestionId, request.UserId, request.SelectedAnswerId));
        
        return Results.Ok();
    }
    
    private record AnswerToQuestionRequest(
        Guid QuestionId, 
        Guid SelectedAnswerId, 
        Guid UserId
    );
}