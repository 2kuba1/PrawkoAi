namespace Application.Models.DTOs;

public record AnswersStatsDto(string CategoryTag, double Accuracy, int TotalAttempts, 
    int SmallErrors, int MediumErrors, int CriticalErrors);