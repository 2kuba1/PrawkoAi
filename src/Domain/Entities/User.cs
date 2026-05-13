using Domain.Shared;

namespace Domain.Entities;

public class User : BaseEntity
{
    public required string DeviceId { get; set; }
    public string? Email { get; set; }
    public Guid RoleId { get; set; }
    public int Streak { get; set; } = 0;
    public DateTime? LastStreakDate { get; set; }

    public List<UserAnswer> UserAnswers { get; set; } = new();
    public Role Role { get; set; }
    public List<ExamSession> ExamSessions { get; set; }
    public List<UserAiProgress> UserAiProgresses { get; set; }
}