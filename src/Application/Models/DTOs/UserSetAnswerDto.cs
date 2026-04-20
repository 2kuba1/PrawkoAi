namespace Application.Models.DTOs;

public record UserSetAnswerDto(
    Guid? SelectedAnswerId,
    Guid QuestionId,
    DateTime AnsweredAt);