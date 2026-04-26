using Application.Features.AI.AnalyzeUserProgress;
using Application.Features.AI.GetAiExplanationStream;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AiEndpoints
{
    public static void MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var aiGroup = app.MapGroup("/api/ai");
        
        aiGroup.MapPost("/aiExplanation", AiQuestionExplanation)
            .RequireAuthorization();

        aiGroup.MapGet("/analyzeUserProgress", AnalyzeUserProgress)
            .RequireAuthorization();;
    }

    private static async Task AiQuestionExplanation([FromBody] AiRequest request, [FromServices] IMediator mediator, HttpContext context, CancellationToken cancellationToken)
    {
        context.Response.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";
        context.Response.Headers["X-Accel-Buffering"] = "no";

        var stream = mediator.CreateStream(new GetAiExplanationStream(
            request.QuestionId,
            request.UserQuery,
            request.Locale
        ), cancellationToken);

        await foreach (var chunk in stream)
        {
            await context.Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
            await context.Response.Body.FlushAsync(cancellationToken);
        }
    }

    private static async Task<IResult> AnalyzeUserProgress([FromQuery] Guid userId, [FromServices] IMediator mediator)
    {
        var response = await mediator.Send(new AnalyzeUserProgress(userId));
        return Results.Ok(response);
    }

    private record AiRequest(Guid QuestionId, string UserQuery, string Locale);
}