using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

internal sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContentPl)
            .IsRequired();
        
        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(x => x.ExamSessionQuestions)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId);
        
        builder.HasOne(x => x.CorrectAnswer)
            .WithMany() 
            .HasForeignKey(x => x.CorrectAnswerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Categories)
            .WithMany(x => x.Questions)
            .UsingEntity(x =>
            {
                x.ToTable("QuestionCategories");

                x.HasIndex("QuestionsId");
                x.HasIndex("CategoriesId");
            });
        
        builder.HasIndex(x => x.ContentPl)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Questions_ContentPl_Trgm");

        builder.HasIndex(x => x.ContentEn)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Questions_ContentEn_Trgm");

        builder.HasIndex(x => x.ContentDe)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Questions_ContentDe_Trgm");

        builder.HasIndex(x => x.ContentUa)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Questions_ContentUa_Trgm");
        
        builder.HasIndex(x => x.QuestionNumber);
        builder.HasIndex(x => x.CategoryType);
        builder.HasIndex(x => x.CategoryTag);
        
    }
}