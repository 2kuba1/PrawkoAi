using Application.Models.DTOs;
using MediatR;

namespace Application.Features.Learning.GetStudyTopics;

public record GetStudyTopics(Guid UserId, string Category) : IRequest<List<GetStudyTopicsResponeDto>>;