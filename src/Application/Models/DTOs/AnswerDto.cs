namespace Application.Models.DTOs;

public record AnswerDto(Guid Id, Guid QuestionId, string Content, DateTime CreatedAt);