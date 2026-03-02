using Application.Models.DTOs;
using Domain.Entities;

namespace Application.Models;

public record StartExamResponse(ExamSessionDto ExamSession, ExamQuestions ExamQuestions);