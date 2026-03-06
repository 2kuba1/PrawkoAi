namespace Application.Models.DTOs;

public class ExamResultsDto
{
    public int CorrectAnswersCount { get; set; }
    public List<AnswerDto> CorrectAnswers { get; set; }
    public List<AnswerDto> NotCorrectAnswers { get; set; }
    public int Score { get; set; }
    public bool IsPassed { get; set; } = false;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}