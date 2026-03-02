using Application.Models.DTOs;
using Domain;
using Domain.Entities;

namespace Application.Models;

public record ExamQuestions(List<QuestionDto> Standard, List<QuestionDto> Specialized);