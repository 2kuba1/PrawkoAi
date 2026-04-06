namespace Application.Models.DTOs;

public record GetQuestionAdditionalDataDto(
    string? MediaUrl,
    string? StaticResponse,
    Guid? CorrectAnswerId
);