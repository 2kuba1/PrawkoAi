using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

public class ExamSessionQuestionConfiguration : IEntityTypeConfiguration<ExamSessionQuestion>
{
    public void Configure(EntityTypeBuilder<ExamSessionQuestion> builder)
    {
        builder.ToTable("ExamSessionQuestions");
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Question)
            .WithMany(x => x.ExamSessionQuestions)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExamSession)
            .WithMany(x => x.ExamSessionQuestions)
            .HasForeignKey(x => x.ExamSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}