using Domain;
using Domain.Entities;

namespace Application.Models;

public record ExamQuestions(List<Question> Standard, List<Question> Specialized);