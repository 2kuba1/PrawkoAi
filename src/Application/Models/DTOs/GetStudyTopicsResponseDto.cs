namespace Application.Models.DTOs;

public record GetStudyTopicsResponseDto(
        string CategoryTag,
        int QuestionsCount,
        int CompletedQuestions
    );