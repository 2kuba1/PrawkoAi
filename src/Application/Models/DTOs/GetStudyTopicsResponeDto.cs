namespace Application.Models.DTOs;

public record GetStudyTopicsResponeDto(
        string CategoryTag,
        int QuestionsCount,
        int CompletedQuestions
    );