using System.Transactions;
using Application.Common;
using Application.Contracts.Repositories;
using Application.Contracts.Services;
using Application.Models.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.Exam.FinishExam;

internal sealed class FinishExamHandler : IRequestHandler<FinishExam, ExamResultsDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExamSessionRepository _examSessionRepository;
    private readonly IExamSessionQuestionRepository _examSessionQuestionRepository;
    private readonly IUserAnswerRepository _userAnswerRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;

    public FinishExamHandler(IHttpContextAccessor httpContextAccessor, IExamSessionRepository examSessionRepository, IExamSessionQuestionRepository examSessionQuestionRepository, IUserAnswerRepository userAnswerRepository, IUserRepository userRepository, IUnitOfWork  unitOfWork, IDistributedCache cache)
    {
        _httpContextAccessor = httpContextAccessor;
        _examSessionRepository = examSessionRepository;
        _examSessionQuestionRepository = examSessionQuestionRepository;
        _userAnswerRepository = userAnswerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    
    public async Task<ExamResultsDto> Handle(FinishExam request, CancellationToken cancellationToken)
    {
        var isHttpContextAndRequestMatching = Utils.GetCurrentUserId(_httpContextAccessor) == request.UserId;
        
        if (!isHttpContextAndRequestMatching)
            throw new UnauthorizedException("You are not allowed to update this exam");
        
        var examSession = await _examSessionRepository.GetByIdAsync(request.ExamSessionId);
        
        if(examSession is null)
            throw new NotFoundException($"Exam session with id {request.ExamSessionId} not found");
 
        if(examSession.UserId != request.UserId)
            throw new UnauthorizedException("You are not authorized to update this exam");

        if (examSession.FinishedAt is not null)
            throw new FinishedExamException("This exam session has been finished");
        
        if ((DateTime.UtcNow - examSession.StaredAt).TotalMinutes > 26)
        {
            examSession.FinishedAt ??= DateTime.UtcNow;
            throw new FinishedExamException("This exam session expired");
        }
        
        var finishedAt = DateTime.UtcNow;
        
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            await _examSessionQuestionRepository.BulkUpdateAnswersAsync(request.ExamSessionId, request.Answers);

            foreach (var userAnswer in request.Answers.Select(answerDto => new UserAnswer
                     {
                         UserId = request.UserId,
                         QuestionId = answerDto.QuestionId,
                         SelectedAnswerId = answerDto.SelectedAnswerId,
                         AnsweredAt = answerDto.AnsweredAt 
                     }))
            {
                await _userAnswerRepository.CreateAsync(userAnswer);
            }
            
            var results =
                await _examSessionQuestionRepository.GetExamResultsAsync(request.ExamSessionId, request.Locale);

            var isPassed = _examSessionRepository.CheckIfPassedAndSaveSession(examSession, finishedAt,
                results.Score, results.CorrectAnswersCount);

            await _userRepository.UpdateStreak(request.UserId, _cache, request.CategoryName);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync();
            
            string cacheKey = $"user_stats_{request.UserId}";
            await _cache.RemoveAsync(cacheKey, cancellationToken);
            
            results.IsPassed = isPassed;
            results.StartedAt = examSession.StaredAt;
            results.FinishedAt = finishedAt;

            return results;
        }
        catch (Exception e)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new TransactionException("An error occurred while updating the exam session");
        }
    }
}