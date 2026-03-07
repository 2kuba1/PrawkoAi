namespace Application.Models.DTOs;

public record ExamSessionHistory(Guid UserId, 
    Guid ExamSessionId, 
    DateTime StartedAt, 
    DateTime? FinishedAt, 
    int? Score, 
    bool IsPassed, 
    int? CorrectAnswersCount);