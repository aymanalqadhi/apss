using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class SurveyEntryTypeConfiguration : IEntityTypeConfiguration<SurveyEntry>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<SurveyEntry> builder)
    {
        builder
            .HasOne(s => s.MadeBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}