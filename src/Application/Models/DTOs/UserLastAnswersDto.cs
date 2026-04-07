namespace Application.Models.DTOs;

public record UserLastAnswersDto(string? CategoryTag, DateTime AnsweredAt, bool WasCorrectlyAnswered, float Points);