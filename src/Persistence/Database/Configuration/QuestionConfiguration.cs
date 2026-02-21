using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.HasMany(x => x.Answers)
            .WithOne(x => x.Question);

        builder.HasOne(x => x.CorrectAnswer)
            .WithOne(x => x.Question)
            .HasForeignKey<Question>(x => x.CorrectAnswerId);
        
        builder.HasMany(x => x.Categories)
            .WithMany(x => x.Questions)
            .UsingEntity(j => j.ToTable("QuestionCategories"));
        
    }
}