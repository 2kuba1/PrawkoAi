namespace Application.Models.DTOs;

public record QuestionDto(Guid Id, string Content, float QuestionNumber, string? CategoryType, float Points, string? MediaUrl, List<AnswerDto> Answers, string? StaticResponse = null);