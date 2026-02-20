using Domain.Shared;

namespace Domain;

public class Answer : BaseEntity
{
    public Guid QuestionId { get; set; }
    public required string Content  { get; set; }

    public Question? Question { get; set; }
    public List<UserAnswer> UserAnswers { get; set; } = new();
}