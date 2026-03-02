using System.Text.Json.Serialization;
using Domain.Shared;

namespace Domain.Entities;

public class ExamSessionQuestion : BaseEntity
{
    public Guid ExamSessionId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public bool? IsCorrect { get; set; }
    public DateTime? AnsweredAt { get; set; }
    
    [JsonIgnore]
    public virtual ExamSession ExamSession { get; set; }
    public virtual Question Question { get; set; }
}