using Application.Features.Category.GetAllCategoriesNames;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class CategoriesEndpoint
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var categoryGroup = app.MapGroup("/api/category");

        categoryGroup.MapGet("/getAllCategoriesNames", GetAllCategoriesNames)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAllCategoriesNames([FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetAllCategoriesNames());
        return Results.Ok(results);
    }
}