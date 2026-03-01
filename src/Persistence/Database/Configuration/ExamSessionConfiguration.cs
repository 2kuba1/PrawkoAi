using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

public class ExamSessionConfiguration : IEntityTypeConfiguration<ExamSession>
{
    public void Configure(EntityTypeBuilder<ExamSession> builder)
    {
        builder.HasKey(o => o.Id);
        builder.ToTable("ExamSessions");

        builder.Property(x => x.StaredAt)
            .IsRequired();
    }
}