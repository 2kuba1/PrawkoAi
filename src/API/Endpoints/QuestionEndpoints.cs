using Application.Features.Questions.GetMediaAndStaticResponse;
using Application.Features.Questions.GetRandomQuestionByCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class QuestionEndpoints
{
    public static void MapQuestionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/questions/getRandomQuestionByCategory", GetRandomQuestionByCategory);
        app.MapGet("/api/questions/getQuestionAdditionalData", GetQuestionAdditionalData);
    }

    private static async Task<IResult> GetRandomQuestionByCategory([FromQuery] string category, IMediator mediator)
    {
        var question = await mediator.Send(new GetRandomQuestionByCategory(category));
        return Results.Ok(question);
    }

    private static async Task<IResult> GetQuestionAdditionalData([FromQuery] Guid questionId, [FromQuery] string locale, [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetQuestionAdditionalData(questionId, locale));
        return Results.Ok(result);
    }
 }