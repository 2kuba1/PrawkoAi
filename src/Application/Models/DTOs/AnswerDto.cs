using Domain.Entities;

namespace Application.Models.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<ExamResultAnswerDto> Answers { get; set; }
    public Guid? SelectedAnswerId { get; set; }
    public float QuestionPoints { get; set; }
    
    public AnswerDto(Guid id, Guid? selectedAnswerId, float questionPoints , Guid questionId, string content, DateTime createdAt, IEnumerable<ExamResultAnswerDto> answers)
    {
        Id = id;
        QuestionId = questionId;
        CreatedAt = createdAt;
        Content = content;
        Answers = answers;
        SelectedAnswerId = selectedAnswerId;
        QuestionPoints =  questionPoints;
    }

    public AnswerDto(Guid id, Guid questionId, string content, DateTime createdAt)
    {
        Id = id;
        QuestionId = questionId;
        CreatedAt = createdAt;
        Content = content;
    }
}