using Application.Contracts.Repositories;
using Application.Models;
using Application.Models.DTOs;
using Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.GetUserExamsSessionHistory;

internal sealed class GetUserExamsSessionHistoryHandler : IRequestHandler<GetUserExamsSessionHistory, PagedList<ExamSessionHistory>>
{
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserExamsSessionHistoryHandler(IExamSessionRepository examSessionRepository, IHttpContextAccessor httpContextAccessor)
    {
        _examSessionRepository = examSessionRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<PagedList<ExamSessionHistory>> Handle(GetUserExamsSessionHistory request, CancellationToken cancellationToken)
    {
        if(Utils.GetCurrentUserId(_httpContextAccessor) != request.UserId)
            throw new UnauthorizedAccessException("You can not access this users exams history");
    
        var results = await _examSessionRepository.GetUserExamsSessionHistory(
            request.UserId, 
            request.PageNumber, 
            15);
        
        return results;
    }
}