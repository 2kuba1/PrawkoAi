using Domain.Shared;

namespace Domain.Entities;

public class Question : BaseEntity
{
    public required string ContentPl { get; set; }
    
    public string? ContentEn { get; set; }
    public string? ContentDe { get; set; }
    public string? ContentUa { get; set; }
    
    public string? AiContext { get; set; }
    public string? StaticResponsePl { get; set; }
    public string? StaticResponseEn { get; set; }
    public string? StaticResponseDe { get; set; }
    public string? StaticResponseUa { get; set; }

    public string? CategoryTag { get; set; }
    public string? CategoryType { get; set; }       
    
    public float QuestionNumber { get; set; }
    public float Points { get; set; }                   
    public string? MediaUrl { get; set; }

    public Guid? CorrectAnswerId { get; set; }
    public Answer? CorrectAnswer { get; set; }

    public virtual List<Category> Categories { get; set; } = new();
    public virtual List<Answer> Answers { get; set; } = new();
    public virtual List<UserAnswer> UserAnswers { get; set; } = new();
    public virtual List<ExamSessionQuestion> ExamSessionQuestions { get; set; }
}