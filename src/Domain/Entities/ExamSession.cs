using Domain.Shared;

namespace Domain.Entities;

public class ExamSession : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime StaredAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
    public int? Score { get; set; }
    public bool IsPassed { get; set; } = false;
    public int CorrectAnswersCount { get; set; } = 0;

    public virtual List<ExamSessionQuestion> ExamSessionQuestions { get; set; }
    public virtual User User { get; set; }
}