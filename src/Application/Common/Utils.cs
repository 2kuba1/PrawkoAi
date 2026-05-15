using Application.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Common;

internal static class Utils
{
    public static Guid? GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        return Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value, out var userId)
            ? userId
            : null;
    }

    public static double CalculatePassProbability(
        List<double> scores,
        List<AnswersStatsDto> answersStats,
        double passThreshold = 68,
        double maxScore = 74)
    {
        if (scores == null || scores.Count == 0)
            return 0;

        scores = scores.TakeLast(5).ToList();

        var avg = scores.Average();

        double trend = 0;
        if (scores.Count >= 2)
            trend = scores.Last() - scores.First();

        double stdDev = 0;
        if (scores.Count >= 2)
        {
            var sumSq = scores.Sum(s => Math.Pow(s - avg, 2));
            stdDev = Math.Sqrt(sumSq / (scores.Count - 1));
        }

        var baseProbability = avg / maxScore;

        var distanceToPass = (avg - passThreshold) / maxScore * 0.5;
        
        var trendBoost = Math.Clamp(trend / 20.0, -0.1, 0.1);

        var stabilityPenalty = stdDev > 6 ? -0.1 : stdDev > 3 ? -0.05 : 0;
        
        double knowledgeBoost = 0;
        double criticalPenalty = 0;
        double weakCategoryPenalty = 0;

        if (answersStats is { Count: > 0 })
        {
            var totalAttempts = (double)answersStats.Sum(x => (int)x.TotalAttempts);

            if (totalAttempts > 0)
            {
                var weightedAccuracy = answersStats.Sum(x =>
                    (double)x.Accuracy * (int)x.TotalAttempts
                ) / totalAttempts;

                var knowledgeScore = weightedAccuracy / 100.0;

                knowledgeBoost = Math.Clamp((knowledgeScore - 0.65) * 0.6, -0.1, 0.2);
                
                var totalCriticalErrors = (double)answersStats.Sum(x => (int)x.CriticalErrors);
                var criticalErrorRate = totalCriticalErrors / totalAttempts;

                criticalPenalty = Math.Clamp(criticalErrorRate * 0.2, 0, 0.1);
                
                var weakCategories = answersStats.Count(x => (double)x.Accuracy < 60);

                weakCategoryPenalty = Math.Clamp(weakCategories * 0.02, 0, 0.08);
            }
        }

        var confidenceFactor = Math.Clamp(scores.Count / 10.0, 0.6, 1.0);
        
        var probability =
            baseProbability +
            distanceToPass +
            trendBoost +
            stabilityPenalty +
            knowledgeBoost -
            criticalPenalty -
            weakCategoryPenalty;

        probability *= confidenceFactor;

        var final = Math.Clamp(probability * 100, 0, 100);

        return Math.Round(final);
    }
}