using Domain.Enums;

namespace Application.Models.DTOs;

public record UserStatsDto(List<AnswersStatsDto> AnswersStats, double? AverageExamScore, ExamTrend ExamTrend, double PassProbability, double TotalAccuracy, string? AiProgressAnalysis);