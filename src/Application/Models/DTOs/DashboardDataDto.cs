namespace Application.Models.DTOs;

public record DashboardDataDto(string? WorstPerformingCategory, int MaxQuestionsCount, int QuestionsAnsweredCount, int Streak, float AverageScore, int TodayQuestionsAnsweredCount);