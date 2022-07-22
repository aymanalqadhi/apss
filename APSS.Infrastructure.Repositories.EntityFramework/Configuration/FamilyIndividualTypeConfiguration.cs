using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class FamilyIndividualTypeConfiguration : IEntityTypeConfiguration<FamilyIndividual>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<FamilyIndividual> builder)
    {
        builder
            .HasOne(f => f.Individual)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}