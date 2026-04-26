using Application.Features.Category.GetAllCategoriesNames;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class CategoriesEndpoint
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();
        
        var categoryGroup = app.MapGroup("/api/v{version:apiVersion}/category")
            .WithApiVersionSet(versionSet);

        categoryGroup.MapGet("/getAllCategoriesNames", GetAllCategoriesNames)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetAllCategoriesNames([FromServices] IMediator mediator)
    {
        var results = await mediator.Send(new GetAllCategoriesNames());
        return Results.Ok(results);
    }
}