using System.Text.Json.Serialization;
using Domain.Shared;

namespace Domain.Entities;

public class Answer : BaseEntity
{
    public Guid QuestionId { get; set; }
    public required string Content  { get; set; }

    [JsonIgnore]
    public Question? Question { get; set; }
    public List<UserAnswer> UserAnswers { get; set; } = new();
}