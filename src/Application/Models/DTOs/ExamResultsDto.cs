namespace Application.Models.DTOs;

public record ExamResultsDto(int CorrectAnswersCount, List<AnswerDto> CorrectAnswers, List<AnswerDto> NotCorrectAnswers, int Score);