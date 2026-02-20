using Domain.Shared;

namespace Domain;

public class UserAnswer : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SelectedAnswerId { get; set; }
    public Guid QuestionId { get; set; }
    public DateTime AnsweredAt { get; set; } =  DateTime.UtcNow;
    
    public User? User { get; set; }
    public Answer? SelectedAnswer { get; set; }
    public Question? Question { get; set; }
}