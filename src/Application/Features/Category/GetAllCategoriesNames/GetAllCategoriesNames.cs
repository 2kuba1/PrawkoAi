using MediatR;

namespace Application.Features.Category.GetAllCategoriesNames;

public record GetAllCategoriesNames() : IRequest<List<string>>;