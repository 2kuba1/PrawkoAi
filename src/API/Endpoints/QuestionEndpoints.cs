using Application.Features.Questions.GetMediaAndStaticResponse;
using Application.Features.Questions.GetRandomQuestionByCategory;
using Application.Features.Questions.SearchForQuestion;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class QuestionEndpoints
{
    public static void MapQuestionsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();
        
        var questionGroup = app.MapGroup("/api/v{version:apiVersion}/questions")
            .WithApiVersionSet(versionSet);
        
        questionGroup.MapGet("/getRandomQuestionByCategory", GetRandomQuestionByCategory)
            .RequireAuthorization();
        questionGroup.MapGet("/getQuestionAdditionalData", GetQuestionAdditionalData)
            .RequireAuthorization();
        questionGroup.MapGet("/search", SearchForQuestion)
            .RequireAuthorization();

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

    private static async Task<IResult> SearchForQuestion([FromQuery] string question, [FromQuery] string locale, [FromQuery] string categoryType, [FromQuery] int pageSize, [FromQuery] int pageNumber, [FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new SearchForQuestion(question, locale, categoryType,  pageSize, pageNumber));
        return Results.Ok(results);
    }
 }