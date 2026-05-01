namespace Application.Models.DTOs;

public record FoundQuestionsDto(
    Guid QuestionId,
    float QuestionNumber,
    string? Content,
    int Points,
    string? CategoryTag
    );