using Domain.Shared;

namespace Domain;

public class Question : BaseEntity
{
    public required string Content { get; set; }
    public float QuestionNumber { get; set; }
    public string? StructureScope { get; set; }       
    public float Points { get; set; }                   
    public string? MediaUrl { get; set; }

    public Guid? CorrectAnswerId { get; set; }
    public Answer? CorrectAnswer { get; set; }

    public List<Category> Categories { get; set; } = new();
    public List<Answer> Answers { get; set; } = new();
    public List<UserAnswer> UserAnswers { get; set; } = new();
}