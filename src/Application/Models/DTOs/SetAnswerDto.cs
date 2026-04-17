namespace Application.Models.DTOs;

public record SetAnswerDto(
    Guid AnswerId,
    string AnswerContent,
    Guid QuestionId);