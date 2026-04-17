namespace Application.Models.DTOs;

public record SetQuestionDto(
    Guid QuestionId,
    float QuestionNumber,
    string QuestionContent, 
    string StaticResponseContent,
    float Points,
    string? MediaUrl,
    Guid? CorrectAnswerId,
    List<SetAnswerDto> Answers);