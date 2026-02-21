using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");
        builder.HasKey(a => a.Id);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasOne(a => a.Question)
            .WithMany(x => x.Answers)
            .HasForeignKey(a => a.QuestionId);
        
        builder.HasMany(a => a.UserAnswers)
            .WithOne(a => a.SelectedAnswer)
            .HasForeignKey(a => a.SelectedAnswerId);
    }
}