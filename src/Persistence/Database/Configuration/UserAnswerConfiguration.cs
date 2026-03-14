using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

internal sealed class UserAnswerConfiguration : IEntityTypeConfiguration<UserAnswer>
{
    public void Configure(EntityTypeBuilder<UserAnswer> builder)
    {
        builder.ToTable("UserAnswers");

        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserAnswers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Question)
            .WithMany(x => x.UserAnswers)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedAnswer)
            .WithMany(x => x.UserAnswers)
            .HasForeignKey(x => x.SelectedAnswerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AnsweredAt)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.QuestionId);
    }
}