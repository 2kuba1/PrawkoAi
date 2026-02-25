using Application.Features.Questions.GetRandomQuestionByCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class QuestionEndpoints
{
    public static void MapQuestionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions/getRandomQuestionByCategory", GetRandomQuestionByCategory);
    }

    private static async Task<IResult> GetRandomQuestionByCategory([FromQuery] string category, IMediator mediator)
    {
        var question = await mediator.Send(new GetRandomQuestionByCategory(category));
        return Results.Ok(question);
    }
}