using Domain.Shared;

namespace Domain;

public class Question : BaseEntity
{
    public required string Content { get; set; }
    public string? MediaUrl { get; set; }
    public string? StaticExplanation { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CorrectAnswerId { get; set; }
    
    public Answer? CorrectAnswer { get; set; }
    public Category? Category { get; set; }
    public List<Answer> Answers { get; set; } = new();

}