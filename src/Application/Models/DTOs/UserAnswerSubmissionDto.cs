namespace Application.Models.DTOs;

public record UserAnswerSubmissionDto(
    Guid QuestionId,
    Guid? SelectedAnswerId,
    DateTime AnsweredAt);