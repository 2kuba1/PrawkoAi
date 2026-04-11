using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Database.Configuration;

internal sealed class UserAiProgressConfiguration : IEntityTypeConfiguration<UserAiProgress>
{
    public void Configure(EntityTypeBuilder<UserAiProgress> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserId)
            .IsRequired();
        
        builder.HasOne(u => u.User)
            .WithMany(u => u.UserAiProgresses)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}