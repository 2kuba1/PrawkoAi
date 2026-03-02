namespace Application.Models.DTOs;

public record QuestionDto(Guid Id, string Content, float QuestionNumber, string? StructureScope, float Points, string? MediaUrl, List<AnswerDto> Answers);