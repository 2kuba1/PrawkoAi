using Application.Contracts.Repositories;
using Application.Models;
using Domain;
using MediatR;

namespace Application.Features.Exam.StartExam;

internal sealed class StartExamHandler : IRequestHandler<StartExam, ExamQuestions>
{
    private readonly IQuestionRepository _questionRepository;

    public StartExamHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<ExamQuestions> Handle(StartExam request, CancellationToken cancellationToken)
    {
        var questions = await _questionRepository.GetExamSimulationQuestions();
        return questions;
    }
}