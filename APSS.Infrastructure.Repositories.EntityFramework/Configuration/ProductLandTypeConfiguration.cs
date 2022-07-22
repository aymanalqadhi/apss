using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APSS.Domain.Entities;

namespace APSS.Infrastructure.Repositories.EntityFramework.Configuration;

public sealed class ProductLandTypeConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasOne(p => p.AddedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}