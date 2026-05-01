using Application.Models;
using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Questions.SearchForQuestion;

public record SearchForQuestion(string Question, string Locale, string CategoryType, int PageSize, int PageNumber) : IRequest<PagedList<FoundQuestionsDto>>;