using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class LandTypeConfiguration : IEntityTypeConfiguration<Land>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Land> builder)
    {
        builder
            .HasMany(l => l.Products)
            .WithOne(p => p.Producer)
            .OnDelete(DeleteBehavior.Restrict);
    }
}